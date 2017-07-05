using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Collections.ObjectModel;
using System.Threading;

namespace Dao.Net {

    public class SocketClient : SocketSession {

        public SocketClient() :
            base(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)) {

        }

        public Task ConnectAsync(IPAddress address, int port) {
            OnAccept();
            return Socket.ConnectTaskAsync(address, port);
        }

        public async Task ConnectAsync(string host, int port) {
            OnAccept();
            await Socket.ConnectTaskAsync(host, port);
            StartReceive();
        }

        public Task DisconnectAsync() {
            return Socket.DisconnectTaskAsync(true);
        }
    }


    public class SyncSocketClient : SocketClient {

        SynchronizationContext _synchronizationContext = SynchronizationContext.Current;

        protected override void Raise(Action action) {
            if (action == null) {
                throw new ArgumentNullException("action");
            }
            if (_synchronizationContext != null) {
                _synchronizationContext.Post(_ => {
                    action();
                }, null);
            } else {
                base.Raise(action);
            }
        }
    }
}
