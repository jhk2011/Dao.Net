using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dao.Net {

    public class SocketSession {

        Socket _socket;

        ISocketConverter _converter;

        public Socket Socket
        {
            get { return _socket; }
        }

        public static int I { get; set; }

        public SocketSession(Socket socket) {
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
            Id = (I++) + "";
            Console.WriteLine("SessionId:{0}", Id);
        }

        public async Task Close() {
            await _socket.DisconnectTaskAsync(false);
            _socket.Close();
        }

        public string Id { get; private set; }

        public event EventHandler<ReceivedEventArgs> Received;

        public event EventHandler Closed;

        public event EventHandler Accepted;

        SocketHandlerCollection handlers = new SocketHandlerCollection();

        public SocketHandlerCollection Handlers { get { return handlers; } }

        public virtual async Task SendAsync(object packet) {

            if (packet == null) throw new ArgumentNullException("packet");

            HandleContext context = new HandleContext {
                Session = this,
                Packet = packet
            };

            Handlers.Send(context);


            await GetConverter().WriteAsync(context.Packet);
        }

        public Task<object> ReceiveAsync() {
            return GetConverter().ReadAsync();
        }

        protected virtual ISocketConverter GetConverter() {
            if (_converter == null) {
                _converter = new SocketConverter(this);
            }
            return _converter;
        }

        public async void StartReceive() {

            object packet = null;
            try {
                packet = await ReceiveAsync();
            } catch (Exception ex) {
                Console.WriteLine("Receive Error:{0}", ex.Message);
                //Console.WriteLine(ex.StackTrace);
                OnClosed();
            }
            if (packet != null) {
                OnReceived(packet);
                StartReceive();
            }
        }

        protected virtual void Raise(Action action) {
            Task.Factory.StartNew(action);
        }

        protected virtual void OnReceived(object packet) {
            Raise(() => {
                HandleContext context = new HandleContext {
                    Packet = packet,
                    Session = this
                };

                Handlers?.Handle(context);
                Received?.Invoke(this, new ReceivedEventArgs(this, packet));
            });
        }

        protected virtual void OnClosed() {
            Raise(() => {
                Handlers?.Close(new HandleContext { Session = this });
                Closed?.Invoke(this, EventArgs.Empty);
            });
        }

        protected virtual void OnAccept() {
            Raise(() => {
                Handlers?.Accept(new HandleContext { Session = this });
                Accepted?.Invoke(this, EventArgs.Empty);
            });
        }
    }

}
