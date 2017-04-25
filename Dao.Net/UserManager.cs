using System;
using System.Collections.Generic;
using System.Linq;

namespace Dao.Net {

    [Serializable]
    public class JoinInfo {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    [Serializable]
    public class JoinReply {
        public int Code { get; set; }
        public string Message { get; set; }
    }

    [Serializable]
    public class LeaveReply {
        public int Code { get; set; }
        public string Message { get; set; }
    }

    public class UserManager : ISocketHandler {
        public string UserName { get; set; }

        public SocketSession Session { get; set; }
        public SocketServer Server { get; set; }

        void ISocketHandler.Handle(Packet packet, SocketSession session) {
            if (packet.Type == UserPackets.Join) {
                var user = packet.GetObject<JoinInfo>();
                var result = DoJoin(user);
                if (result.Code == 0) {
                    UserName = user.UserName;
                }
                Packet reply = new Packet(UserPackets.JoinReply);
                reply.SetObject(result);
                session.SendAsync(reply);
            } else if (packet.Type == UserPackets.JoinReply) {
                JoinReply reply = packet.GetObject<JoinReply>();

                Join?.Invoke(reply);

            }
            if (packet.Type == UserPackets.Leave) {
                Packet reply = new Packet(UserPackets.LeaveReply);
                session.SendAsync(reply);
            } else if (packet.Type == UserPackets.LeaveReply) {
                LeaveReply obj = new LeaveReply();
                Leave?.Invoke(obj);
            } else if (packet.Type == UserPackets.GetUsers) {

                var users = DoGetUsers();
                Packet reply = new Packet(UserPackets.GetUsersReply);

                reply.SetObject(users);

                session.SendAsync(reply);

            } else if (packet.Type == UserPackets.GetUsersReply) {
                GetUsers?.Invoke(packet.GetObject<List<String>>());
            }
        }

        public event Action<JoinReply> Join;

        public event Action<LeaveReply> Leave;

        public event Action<List<String>> GetUsers;

        public void JoinAsync(string username, string password) {
            UserName = username;
            Packet packet = new Packet(UserPackets.Join);
            packet.SetObject(new JoinInfo() {
                UserName = username,
                Password = password
            });
            Session.SendAsync(packet);
        }

        public void LeaveAsync() {
            Packet packet = new Packet(UserPackets.Leave);
            Session.SendAsync(packet);
        }

        public void GetUserAsync() {
            Packet p = new Packet(UserPackets.GetUsers);
            Session.SendAsync(p);
        }


        public List<string> DoGetUsers() {
            return Server.Sessions
               .Select(x => x.Handlers
                   .OfType<UserManager>()
                   .FirstOrDefault())
               .Select(x => x.UserName)
               .ToList();
        }


        protected JoinReply DoJoin(JoinInfo user) {
            return new JoinReply {
                Code = 0,
                Message = "成功"
            };
        }
    }
}
