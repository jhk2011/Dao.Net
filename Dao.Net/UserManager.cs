using System;
using System.Collections.Generic;
using System.Linq;

namespace Dao.Net {

    [Serializable]
    public class UserLoginInfo {
        public string UserId { get; set; }
        public string Password { get; set; }
    }

    public class UserManager : ISocketHandler {

        public SocketSession Session { get; set; }
        public SocketServer Server { get; set; }

        public string UserId { get; set; }

        void ISocketHandler.Handle(Packet packet, SocketSession session) {
            if (packet.Type == UserPackets.Login) {
                var user = packet.GetObject<UserLoginInfo>();
                var result = Login(user);
                if (result) {
                    UserId = user.UserId;
                }
                Packet reply = new Packet(UserPackets.LoginReply);
                reply.SetObject(result);
                session.SendAsync(reply);
            } else if (packet.Type == UserPackets.LoginReply) {
                bool value = packet.GetObject<bool>();
                OnLoginCompleted(value);
            } else if (packet.Type == UserPackets.GetUsers) {
                var users = GetUsers();
                Packet reply = new Packet(UserPackets.GetUsersReply);
                reply.SetObject(users);
                session.SendAsync(reply);
            } else if (packet.Type == UserPackets.GetUsersReply) {
                GetUsersCompleted?.Invoke(packet.GetObject<List<String>>());
            }
        }

        private void OnLoginCompleted(bool value) {
            if (value) {
                UserId = LastUserId;
            }
            LoginCompleted?.Invoke(value);
        }

        public event Action<List<String>> GetUsersCompleted;

        public string LastUserId { get; set; }
        public void LoginAsync(string userid, string password) {
            LastUserId = userid;
            Packet packet = new Packet(UserPackets.Login);
            packet.SetObject(new UserLoginInfo() {
                UserId = userid,
                Password = password
            });
            Session.SendAsync(packet);
        }

        public void GetUserAsync() {
            Packet p = new Packet(UserPackets.GetUsers);
            Session.SendAsync(p);
        }

        public List<string> GetUsers() {
            return Server.Sessions
                .Select(x => x.Handlers
                    .OfType<UserManager>()
                    .FirstOrDefault())
                .Select(x => x.UserId)
                .ToList();
        }

        public event Action<bool> LoginCompleted;

        protected bool Login(UserLoginInfo user) {
            return true;
        }
    }
}
