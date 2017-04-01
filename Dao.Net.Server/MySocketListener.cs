using System;
using System.Net.Sockets;

namespace Dao.Net.Server {
    partial class Program {
        class MySocketListener :SocketListener {
            protected override void OnAccepted(Socket socket) {
                //Console.WriteLine("OnAccepted");
                base.OnAccepted(socket);
            }
        }
    }
}
