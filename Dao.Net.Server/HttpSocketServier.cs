using System.Net.Sockets;

namespace Dao.Net.Server {
    partial class Program {
        public class HttpSocketServier : SocketServer {
            protected override SocketSession GetSession(Socket client) {
                return new HttpSocketSession(client);
            }

        }

    }
}
