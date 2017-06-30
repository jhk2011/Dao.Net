using System.Net.Sockets;

namespace Dao.Net.Server {
    class MySocketServer : SyncSocketServer {

        public MySocketServer() {

        }

        public MySocketServer(SocketListener l) : base(l) {

        }

        protected override SocketSession GetSocketSession(Socket client) {
            return new MyServerSession(client, this);
        }

        protected override void OnAccepted(SocketSession session) {
            base.OnAccepted(session);
        }

        protected override void OnClosed(SocketSession session) {

            base.OnClosed(session);
        }

        protected override void OnReceived(SocketSession session, Packet packet) {
            base.OnReceived(session, packet);
        }
    }
}
