using System.IO;
using System.Linq;
using System.Text;

namespace Dao.Net.Server {
    internal class MyServerHandler : ISocketHandler {
        public   void Handle(Packet packet, SocketSession session) {
            // session.SendAsync(packet);
        }
    }

}