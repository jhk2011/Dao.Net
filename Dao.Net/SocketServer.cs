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

        public SocketListener Listener { get { return _listener; } }

        SocketSessionCollection _sessions = new SocketSessionCollection();
        public SocketSessionCollection Sessions { get { return _sessions; } }

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

        protected virtual SocketSession GetSession(Socket client) {
            return new SocketSession(client);
        }

        private void OnAccepted(Socket client) {
            SocketSession session = GetSession(client);
            OnAccepted(session);
        }

        protected virtual void OnAccepted(SocketSession session) {
            session.Closed += OnClosed;
            session.StartReceive();
            _sessions.Add(session);

            session.Handlers?.Accept(new HandleContext { Session = session });
        }

        private void OnClosed(object sender, EventArgs e) {
            SocketSession session = sender as SocketSession;
            OnClosed(session);
        }

        protected virtual void OnClosed(SocketSession session) {
            session.Closed -= OnClosed;
            _sessions.Remove(session);
            //session.Handlers?.Close(new HandleContext { Session = session });
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
        protected override void OnClosed(SocketSession session) {
            Raise(() => {
                base.OnClosed(session);
            });
        }
    }

}
