using System.Net.Sockets;
using System.Threading.Tasks;

namespace Dao.Net.Server {
    class MyServerSession : SocketSession {

        public MySocketServer Server { get; set; }

        public MyServerSession(Socket socket, MySocketServer server) : base(socket) {
            Server = server;
            this.Handlers = new PacketHandlerCollection();

            Handlers.Add(new UserManager {
                Server = server,
                Session = this
            });

            Handlers.Add(new TransferManager() {
                Server = server
            });

            //Handlers.Add(new MyServerHandler());
            //Handlers.Add(new UserManager() {
            //    Session = this,
            //    Server = server
            //});
            //Handlers.Add(new FileManager(this));
            //Handlers.Add(new TerminalServerManager { Session = this });

            ServiceManager serviceManager = new ServiceManager();

            //h.AddService("calc", new Calc());
            //h.Session = this;

            Handlers.Add(serviceManager);
        }

        protected override void OnReceived(Packet packet) {
            Task.Factory.StartNew(()=> {
                base.OnReceived(packet);
            });
            //System.Console.WriteLine("OnReceived:{0}", packet.Type);
            
        }
    }
}
