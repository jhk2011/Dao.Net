using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Dao.Net {
    public class SocketHandlerCollection : Collection<ISocketHandler>, ISocketHandler {

        public void Accept(HandleContext context) {
            foreach (var handler in this) {
                if (context.Cancel) break;
                handler?.Accept(context);
            }
        }

        public void Close(HandleContext context) {
            foreach (var handler in this) {
                if (context.Cancel) break;
                handler?.Close(context);
            }
        }

        public void Handle(HandleContext context) {
            foreach (var handler in this) {
                if (context.Cancel) break;
                handler?.Handle(context);
            }
        }

        public void Send(HandleContext context) {
            foreach (var handler in this) {
                if (context.Cancel) break;
                handler?.Send(context);
            }
        }
    }
}
