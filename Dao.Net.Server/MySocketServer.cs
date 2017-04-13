using System.Net.Sockets;

namespace Dao.Net.Server {
    class MySocketServer : SocketServer {

        public MySocketServer() {

        }

        public MySocketServer(SocketListener l) : base(l) {

        }

        protected override SocketSession GetSocketSession(Socket client) {
            return new MyServerSession(client, this);
        }

        protected override void OnAccepted(SocketSession session) {
            base.OnAccepted(session);
            System.Console.WriteLine(session.Socket.RemoteEndPoint);
        }

        protected override void OnReceived(SocketSession session, Packet packet) {
            base.OnReceived(session, packet);
        }
    }
}
