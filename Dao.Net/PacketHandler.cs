namespace Dao.Net {

    public class HandleContext {
        public Packet Packet { get; set; }
        public SocketSession Session { get; set; }
        public bool Cancel { get; set; }
    }
    public interface ISocketHandler {
        void Handle(Packet packet, SocketSession session);
    }
}
