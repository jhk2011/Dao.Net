using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Dao.Net.Client {
    class ClientSocketSession : SocketSession {
        public string DestUserId { get; set; }

        public TerminalClientManager TerminalManager { get; set; }

        public SocketClient Client { get; set; }
        public ClientSocketSession(Socket socket) : base(socket) {
            this.Handlers = new PacketHandlerCollection();
            TerminalManager = new TerminalClientManager() {
                Session = this
            };
            this.Handlers.Add(TerminalManager);
        }

        public override Task SendAsync(Packet packet) {
            packet.ScrUserId = Client.Handlers
                .GetHandler<UserManager>()
                ?.UserName;

            packet.DestUserId = DestUserId;
            return base.SendAsync(packet);
        }

        protected override void OnReceived(Packet packet) {
            var username = Client.Handlers
                .GetHandler<UserManager>()
                ?.UserName;

            //if (packet.DestUserId != username) {
            //    throw new InvalidOperationException("");
            //}

            if (packet.ScrUserId == DestUserId) {
                base.OnReceived(packet);
            }
        }

        internal void Receive(Packet packet) {
            OnReceived(packet);
        }
    }
}
