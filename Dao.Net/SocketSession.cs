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
        IPacketConverter _converter;

        public Socket Socket
        {
            get { return _socket; }
            private set { _socket = value; }
        }

        public SocketSession(Socket socket, IPacketConverter converter) {
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        public SocketSession(Socket socket) : this(socket, PacketConverter.Default) {

        }

        public event EventHandler<ReceivedEventArgs> Received;

        public event EventHandler Closed;

        public SocketHandlerCollection Handlers { get; set; }

        public async Task SendAsync(Packet packet) {
            if (packet == null) throw new ArgumentNullException("buffer");
            await _converter.SendAsync(this, packet);
        }

        public async Task<Packet> ReceiveAsync() {
            return await _converter.ReceiveAsync(this);
        }

        public async void StartReceive() {

            Packet packet = null;
            try {
                packet = await ReceiveAsync();
            } catch (Exception ex) {
                Console.WriteLine("Receive Error:{0}", ex.Message);
                Console.WriteLine(ex.StackTrace);
                OnClosed();
            }
            if (packet != null) {
                OnReceived(packet);
                StartReceive();
            }
        }

        protected virtual void OnReceived(Packet packet) {
            Task.Factory.StartNew(() => {
                try {
                    SocketContext.Current = new SocketContext {
                        Packet = packet,
                        Session = this
                    };

                    HandleContext context = new HandleContext {
                        Packet = packet,
                        Session = this
                    };

                    Received?.Invoke(this, new ReceivedEventArgs(this, packet));

                    Handlers?.Handle(context);

                } finally {
                    SocketContext.Current = null;
                }
            });
        }

        protected virtual void OnClosed() {
            Closed?.Invoke(this, EventArgs.Empty);
        }
    }

}
