using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Dao.Net {

    [Serializable]
    public class ClientCloseInfo {
        public string SrcUserId { get; set; }
    }

    /// <summary>
    /// 服务端使用，负责转发数据
    /// </summary>
    public class TransferHandler : SocketHandler {

        public SocketServer Server { get; set; }

        public override async void Close(HandleContext context) {

            Console.WriteLine("----Close");

            foreach (var session in sessions) {
                await session.Key.SendAsync(new ClientCloseInfo {
                    SrcUserId = context.Session.Id
                });
            }
        }

        /// <summary>
        /// 转发过消息的客户端
        /// </summary>
        Dictionary<SocketSession, object> sessions = new Dictionary<SocketSession, object>();

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

                if (!sessions.ContainsKey(session)) {
                    sessions.Add(session, null);
                }

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

        protected virtual SocketSession Find(ITransferable info) {
            return Server.Sessions
                .Where(x => x.Id == info.DestUserId)
                .FirstOrDefault();
        }
    }

    /// <summary>
    /// 客户端使用，将收到的来自不同客户端的消息进行分发，
    /// 并在发送时加上目标
    /// </summary>
    public class TransferClientHandler : SocketHandler {

        SocketSessionCollection sessions = new SocketSessionCollection();

        public override void Handle(HandleContext context) {
            var packet = context.Packet;


            var closeInfo = context.Packet as ClientCloseInfo;

            if (closeInfo != null) {

                context.Cancel = true;

                string userId2 = closeInfo.SrcUserId;
                bool value = false;
                SocketSession session2 = GetSession(context, userId2, ref value);
                session2.Handlers.Close(new HandleContext { Session = session2 });

                Console.WriteLine("----客户端关闭");

            } else {
                var transfer = context.Packet as ITransferable;

                if (transfer != null) {

                    context.Cancel = true;

                    string userId = transfer.SrcUserId;

                    bool accept = false;

                    SocketSession session = GetSession(context, userId, ref accept);

                    session.Handlers.Handle(new HandleContext {
                        Session = session,
                        Packet = packet
                    });
                    
                }
            }

        }

        private SocketSession GetSession(HandleContext context, string userId, ref bool accept) {
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

            return session;
        }

        protected virtual SocketSession GetSession(Socket socket, string userId) {
            return new TransferSession(socket, userId);
        }
    }

    /// <summary>
    /// 将发送数据加上目标
    /// </summary>
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
