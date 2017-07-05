using System;

namespace Dao.Net.Client {
    public class SocketClientHandler : SocketHandler {
        public override void Handle(HandleContext context) {
            Console.WriteLine(context.Packet.GetType().FullName);
        }
    }
}
