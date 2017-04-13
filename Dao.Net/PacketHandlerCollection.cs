using System.Collections.ObjectModel;
using System.Linq;

namespace Dao.Net {
    public class PacketHandlerCollection : Collection<ISocketHandler> {
        public void Handle(Packet packet, SocketSession session) {
            foreach (var handler in this) {
                try {
                    handler?.Handle(packet, session);
                } catch(BreakException) {
                    break;
                }
            }
        }

        public T GetHandler<T>() {
            return this.OfType<T>().FirstOrDefault();
        }
    }
}
