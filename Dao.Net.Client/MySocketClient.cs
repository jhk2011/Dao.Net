using System.IO;
using System.Text;

namespace Dao.Net.Client {
    class MySocketClient : SocketClient {

        public UserManager UserManager { get; set; }
        public FileManager FileManager { get; set; }

        public TerminalManager TerminalManager { get; set; }

        public MySocketClient() {
            this.Handlers = new PacketHandlerCollection();
            this.Handlers.Add(new SocketClientHandler());

            UserManager = new UserManager() { Session = this };
            FileManager = new FileManager(this);
            TerminalManager = new TerminalManager { Session = this };

            this.Handlers.Add(UserManager);
            this.Handlers.Add(FileManager);
            this.Handlers.Add(TerminalManager);
            this.Handlers.Add(new ServiceManager {
                Session = this
            });
        }

        protected override void OnReceived(Packet packet) {
            //Console.WriteLine("OnReceived:{0}", packet.Type);
            base.OnReceived(packet);
        }
    }
}
