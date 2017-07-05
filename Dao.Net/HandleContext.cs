namespace Dao.Net {

    public class HandleContext {
        public object Packet { get; set; }
        public SocketSession Session { get; set; }

        public bool Cancel { get; set; }
    }
}
