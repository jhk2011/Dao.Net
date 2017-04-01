using System.Threading.Tasks;

namespace Dao.Net {
    public interface IPacketConverter {
        Task<Packet> ReceiveAsync(SocketSession session);
        Task SendAsync(SocketSession session, Packet packet);
    }
}