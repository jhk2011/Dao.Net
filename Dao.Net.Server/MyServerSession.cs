using System.Net.Sockets;

namespace Dao.Net.Server {
    class MyServerSession : SocketSession {

        public MySocketServer Server { get; set; }

        public MyServerSession(Socket socket, MySocketServer server) : base(socket) {
            Server = server;
            this.Handlers = new PacketHandlerCollection();
            Handlers.Add(new TransferManager() {
                Server = server
            });
            Handlers.Add(new MyServerHandler());
            Handlers.Add(new UserManager() {
                Session = this,
                Server = server
            });
            Handlers.Add(new FileManager(this));
            Handlers.Add(new TerminalManager { Session = this });

            ServiceManager h = new ServiceManager();

            h.AddService("calc", new Calc());
            h.Session = this;

            Handlers.Add(h);
        }

        protected override void OnReceived(Packet packet) {
            //System.Console.WriteLine("OnReceived:{0}", packet.Type);
            base.OnReceived(packet);
        }
    }
}
