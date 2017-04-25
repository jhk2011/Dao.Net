using System;

namespace Dao.Net.Client {
    public class SocketClientHandler : ISocketHandler {
        public  void Handle(Packet packet, SocketSession session) {
            Console.WriteLine(packet.Type);
        }
    }
}
