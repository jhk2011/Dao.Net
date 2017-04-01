namespace Dao.Net {
    public interface ISocketHandler {
        void Handle(Packet packet, SocketSession session);
    }
}
