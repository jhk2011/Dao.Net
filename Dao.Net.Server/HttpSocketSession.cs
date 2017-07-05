using System.Net.Sockets;
using Dao.Net.Web;

namespace Dao.Net.Server {
    partial class Program {
        public class HttpSocketSession : SocketSession {

            public HttpSocketSession(Socket socket) : base(socket) {
                this.Handlers.Add(new HttpHandler());
            }

            protected override ISocketConverter GetConverter() {
                return new HttpConverter(this);
            }

        }

    }
}
