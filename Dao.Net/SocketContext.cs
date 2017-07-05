using System.Threading;

namespace Dao.Net {
    public class SocketContext {

        static ThreadLocal<SocketContext> current = new ThreadLocal<SocketContext>();

        public static SocketContext Current
        {
            get
            {
                return current.Value;
            }

            internal set
            {
                current.Value = value;
            }
        }

        public object Packet { get; set; }

        public SocketSession Session { get; set; }

    }
}
