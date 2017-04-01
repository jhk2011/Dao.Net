using System.Collections.ObjectModel;

namespace Dao.Net {
    public class PacketHandlerCollection : Collection<ISocketHandler> {
        public void Handle(Packet packet, SocketSession session) {
            foreach (var handler in this) {
                handler?.Handle(packet, session);
            }
        }
    }
}
