using System.Collections.ObjectModel;
using System.Linq;

namespace Dao.Net {
    public class SocketHandlerCollection : Collection<ISocketHandler> {
        public void Handle(HandleContext context) {
            foreach (var handler in this) {
                handler?.Handle(context);
            }
        }
    }
}
