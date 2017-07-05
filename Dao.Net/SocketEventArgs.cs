using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Net
{
    public class AcceptdEventArgs : EventArgs
    {
        public AcceptdEventArgs(SocketSession session)
        {
            Session = session;
        }
        public SocketSession Session { get; private set; }
    }

    public class ClosedEventArgs : EventArgs
    {
        public ClosedEventArgs(SocketSession session)
        {
            Session = session;
        }
        public SocketSession Session { get; private set; }
    }

    public class ReceivedEventArgs : EventArgs
    {
        public ReceivedEventArgs(SocketSession session, object packet)
        {
            Session = session;
            Packet = packet;
        }

        public SocketSession Session { get; private set; }

        public object Packet { get; private set; }
    }

}
