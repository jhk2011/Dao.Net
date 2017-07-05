using System.Net.Sockets;
using Dao.Net.Web;

namespace Dao.Net.Web {
  
        public class HttpSession : SocketSession {

            public HttpSession(Socket socket) : base(socket) {
                this.Handlers.Add(new HttpHandler());
            }

            protected override ISocketConverter GetConverter() {
                return new HttpConverter(this);
            }
        }

}
