using System.IO;
using System.Text;

namespace Dao.Net.Client {
    class MySocketClient : SocketClient {

        public UserManager UserManager { get; set; }
        public FileManager FileManager { get; set; }

        public MySocketClient() {
            this.Handlers = new PacketHandlerCollection();
            this.Handlers.Add(new SocketClientHandler());

            UserManager = new UserManager() { Session = this };
            FileManager = new FileManager(this);

            this.Handlers.Add(UserManager);
            this.Handlers.Add(FileManager);
        }

        protected override void OnReceived(Packet packet) {
            //Console.WriteLine("OnReceived:{0}", packet.Type);
            base.OnReceived(packet);
        }
    }
}
