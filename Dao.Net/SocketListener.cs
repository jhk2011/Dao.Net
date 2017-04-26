using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Net {
    public class SocketListener {

        Socket _socket;

        public Socket Socket
        {
            get { return _socket; }
        }

        public event Action<Socket> Accepted;

        public event Action<Exception> Error;

        public void Initialize(int port) {
            Initialize(port, 20);
        }

        public void Initialize(int port, int backlog) {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
            _socket.Listen(backlog);
        }

        public Task<Socket> AcceptAsync() {
            return _socket.AcceptTaskAsync();
        }

        public async void StartAccept() {

            Socket client = null;

            try {
                client = await this.AcceptAsync().ConfigureAwait(false);
            } catch (Exception ex) {
                Console.WriteLine("Accept Error:{0}", ex.Message);
                OnError(ex);
            }

            if (client != null) {
                OnAccepted(client);
                StartAccept();
            }
        }

        protected virtual void OnAccepted(Socket socket) {
            Accepted?.Invoke(socket);
        }

        protected virtual void OnError(Exception error) {
            Error?.Invoke(error);
        }

    }
}
