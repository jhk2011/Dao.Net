using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Dao.Net.Server {
    internal class MyServerHandler : SocketHandler {
        public override void Handle(HandleContext context) {
            var p = context.Packet;
            if (p == null) return;
            Console.WriteLine("{0}({1})", p, p.GetType().Name);
        }

        public override void Accept(HandleContext context) {
            Console.WriteLine("Accept:{0}", context.Session.Socket.RemoteEndPoint);
        }

        public override void Close(HandleContext context) {
            Console.WriteLine("Close:{0}", context.Session.Socket.RemoteEndPoint);
        }
    }

}