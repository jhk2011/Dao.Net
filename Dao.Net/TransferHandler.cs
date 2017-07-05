using System;
using System.Linq;

namespace Dao.Net {
    public class TransferHandler : SocketHandler {

        public SocketServer Server { get; set; }

        public override async void Handle(HandleContext context) {

            var packet = context.Packet;

            var transfer = context.Packet as ITransferable;

            if (transfer == null) return;


            if (string.IsNullOrEmpty(transfer.DestUserId)) return;

            context.Cancel = true;

            transfer.SrcUserId = context.Session.Id;

            Console.WriteLine("中转消息:{0}->{1}", transfer.SrcUserId, transfer.DestUserId);

            SocketSession session = Find(transfer);

            if (session != null) {
                await session.SendAsync(transfer);
            } else {

                var request = packet as IRequest;

                if (request != null) {

                    Response response = new Response {
                        Id = request.Id,
                        Message = "客户端不在线"
                    };
                    await context.Session.SendAsync(response);
                }
            }

        }

        private SocketSession Find(ITransferable info) {
            return Server.Sessions
                .Where(x => x.Id == info.DestUserId)
                .FirstOrDefault();
        }
    }
}
