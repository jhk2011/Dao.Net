using System;

namespace Dao.Net {

    [Serializable]
    public class UserLoginInfo {
        public string UserId { get; set; }
        public string Password { get; set; }
    }

    public class UserManager : ISocketHandler {

        public SocketSession Session { get; set; }

        public string UserId { get; set; }

        void ISocketHandler.Handle(Packet packet, SocketSession session) {
            if (packet.Type == UserPackets.Login) {
                var user = packet.GetObject<UserLoginInfo>();
                var result = Login(user);
                Packet reply = new Packet(UserPackets.LoginReply);
                reply.SetObject(result);
                session.SendAsync(reply);
            } else if (packet.Type == UserPackets.LoginReply) {
                bool value = packet.GetObject<bool>();
                LoginCompleted?.Invoke(value);
            }
        }

        public void LoginAsync(string userid, string password) {
            Packet packet = new Packet(UserPackets.Login);
            packet.SetObject(new UserLoginInfo() {
                UserId = userid,
                Password = password
            });
            Session.SendAsync(packet);
        }

        public event Action<bool> LoginCompleted;

        protected bool Login(UserLoginInfo user) {
            return true;
        }
    }
}
