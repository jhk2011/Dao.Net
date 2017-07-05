using System.Net.Sockets;

namespace Dao.Net.Server {
    class MySocketServer : SyncSocketServer {

        public MySocketServer() {

        }

        public MySocketServer(SocketListener l) : base(l) {
            
        }

        protected override SocketSession GetSession(Socket client) {
            return new MyServerSession(client, this);
        }

    }
}
