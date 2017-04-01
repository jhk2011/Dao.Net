using System.Net.Sockets;

namespace Dao.Net.Server {
    class MyServerSession : SocketSession {

        public MyServerSession(Socket socket) : base(socket) {
            this.Handlers = new PacketHandlerCollection();
            Handlers.Add(new MyServerHandler());
            Handlers.Add(new UserManager() { Session = this });
            Handlers.Add(new FileManager(this));
        }

        protected override void OnReceived(Packet packet) {
            //System.Console.WriteLine("OnReceived:{0}", packet.Type);
            base.OnReceived(packet);
        }
    }
}
