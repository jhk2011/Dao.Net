using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

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

    public class TransferClientHandler : SocketHandler {

        SocketSessionCollection sessions = new SocketSessionCollection();

        public override void Handle(HandleContext context) {
            var packet = context.Packet;

            var transfer = context.Packet as ITransferable;

            string userId = transfer?.SrcUserId;

            bool accept = false;

            SocketSession session = null;

            lock (sessions.SyncObj) {

                session = sessions.Where(x => x.Id == userId).FirstOrDefault();

                if (session == null) {
                    session = GetSession(context.Session.Socket, userId);

                    sessions.Add(session);

                    accept = true;
                }
            }

            if (accept) {
                session.Handlers.Accept(new HandleContext {
                    Session = session
                });
            }

            session.Handlers.Handle(new HandleContext {
                Session = session,
                Packet = packet
            });
        }

        protected virtual SocketSession GetSession(Socket socket, string userId) {
            return new TransferSession(socket, userId);
        }
    }

    public class TransferSession : SocketSession {


        public TransferSession(Socket socket, string userid) : base(socket) {
            this.Id = userid;
        }

        public override Task SendAsync(object packet) {

            var t = packet as ITransferable;

            if (t != null) {
                t.DestUserId = this.Id;
            }

            return base.SendAsync(packet);
        }
    }
}
