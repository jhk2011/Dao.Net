using System.Net.Sockets;

namespace Dao.Net.Web {
    public class HttpServer : SocketServer {
        protected override SocketSession GetSession(Socket client) {
            return new HttpSession(client);
        }
    }
}
