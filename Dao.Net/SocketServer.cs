using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Dao.Net {

    public class SocketServer {

        SocketListener _listener;

        public SocketListener Listener
        {
            get { return _listener; }
            //set { _listener = value; }
        }

        public event EventHandler<AcceptdEventArgs> Accepted;

        public event EventHandler<ReceivedEventArgs> Received;

        public event EventHandler<ClosedEventArgs> Closed;

        public SocketServer(SocketListener listener) {
            if (listener == null) {
                throw new ArgumentNullException("listener");
            }
            _listener = listener;
        }

        public SocketServer()
            : this(new SocketListener()) {

        }

        public void Initialize(int port) {
            Initialize(port, 100);
        }

        public void Initialize(int port, int backlog) {
            _listener.Accepted += OnAccepted;
            _listener.Initialize(port, backlog);
            _listener.StartAccept();
        }

        protected virtual SocketSession GetSocketSession(Socket client) {
            return new SocketSession(client);
        }

        SocketSessionCollection _sessions = new SocketSessionCollection();

        public SocketSessionCollection Sessions
        {
            get { return _sessions; }
        }

        private void OnAccepted(Socket client) {
            SocketSession session = GetSocketSession(client);
            OnAccepted(session);
        }

        protected virtual void OnAccepted(SocketSession session) {
            session.Received += OnReceived;
            session.Closed += OnClosed;
            session.StartReceive();
            _sessions.Add(session);

            Accepted?.Invoke(this, new AcceptdEventArgs(session));
        }

        private void OnReceived(object sender, ReceivedEventArgs e) {
            SocketSession session = sender as SocketSession;
            OnReceived(session, e.Pakcet);
        }

        protected virtual void OnReceived(SocketSession session, Packet packet) {
            Received?.Invoke(this, new ReceivedEventArgs(session, packet));
        }
        private void OnClosed(object sender, EventArgs e) {
            SocketSession session = sender as SocketSession;
            OnClosed(session);
        }

        protected virtual void OnClosed(SocketSession session) {
            session.Received -= OnReceived;
            session.Closed -= OnClosed;
            _sessions.Remove(session);

            Closed?.Invoke(this, new ClosedEventArgs(session));
        }
    }

    /// <summary>
    /// 在线程的同步上下文引发事件
    /// </summary>
    public class SyncSocketServer : SocketServer {

        public SyncSocketServer() {

        }

        public SyncSocketServer(SocketListener l) : base(l) {

        }

        SynchronizationContext _syncContext = SynchronizationContext.Current;

        protected virtual void Raise(Action action) {
            if (action == null) throw new ArgumentNullException("action");
            if (_syncContext == null) {
                Task.Factory.StartNew(action);
            } else {
                _syncContext.Post(_ => action(), null);
            }
        }

        protected override void OnAccepted(SocketSession session) {
            Raise(() => {
                base.OnAccepted(session);
            });
        }

        protected override void OnReceived(SocketSession session, Packet packet) {
            Raise(() => {
                base.OnReceived(session, packet);
            });
        }

        protected override void OnClosed(SocketSession session) {
            Raise(() => {
                base.OnClosed(session);
            });
        }
    }

}
