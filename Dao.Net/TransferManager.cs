using System;
using System.Linq;

namespace Dao.Net {
    public class TransferManager : ISocketHandler {

        public SocketServer Server { get; set; }

        public void Handle(Packet packet, SocketSession session) {
            if (!string.IsNullOrEmpty(packet.DestUserId)) {
                SocketSession session2 = FindSocketSession(packet.DestUserId);
                if (session2 != null) {
                    Console.WriteLine("转发数据:{0}->{1} 类型{2}",
                        packet.ScrUserId,
                        packet.DestUserId,
                        packet.Type);

                    session2.SendAsync(packet);
                }
                throw new BreakException();
            }
        }

        private SocketSession FindSocketSession(string destUserId) {
            return Server.Sessions
                .Select(x => x.Handlers.GetHandler<UserManager>())
                .Where(x => x.UserName == destUserId)
                .Select(x => x.Session)
                .FirstOrDefault();
        }
    }
}
