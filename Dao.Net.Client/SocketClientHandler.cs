using System;

namespace Dao.Net.Client {
    public class SocketClientHandler : ISocketHandler {
        public  void Handle(Packet packet, SocketSession session) {
            if (packet.Type == FilePackets.GetFiles) {
                Console.WriteLine(packet.GetString());
            }
        }
    }
}
