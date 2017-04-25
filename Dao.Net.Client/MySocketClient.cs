using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dao.Net.Client {


    class MySocketClient : SocketClient {

        public UserManager UserManager { get; set; }
        public FileManager FileManager { get; set; }

        public TerminalClientManager TerminalManager { get; set; }


        public MySocketClient() {
            this.Handlers = new PacketHandlerCollection();

            this.Handlers.Add(new SocketClientHandler());

            //SystemManager systemManager = new SystemManager {
            //    Session = this
            //};
            //systemManager.Join += OnJoin;
            //systemManager.Leave += OnLeave;

            //this.Handlers.Add(systemManager);

            UserManager = new UserManager() { Session = this };

            //FileManager = new FileManager(this);
            TerminalManager = new TerminalClientManager { Session = this };

            this.Handlers.Add(UserManager);
            //this.Handlers.Add(FileManager);

            this.Handlers.Add(TerminalManager);

            this.Handlers.Add(new TerminalServerManager() {
                Session = this
            });

            //this.Handlers.Add(new ServiceManager {
            //    Session = this
            //});
        }

        List<ClientSocketSession> sessions = new List<ClientSocketSession>();

        public virtual ClientSocketSession GetSession(string obj) {
            var session = new ClientSocketSession(this.Socket) {
                DestUserId = obj,
                Client = this
            };
            sessions.Add(session);
            return session;
        }


        protected override void OnReceived(Packet packet) {
            var sessions2 = sessions.Where(x => x.DestUserId == packet.DestUserId);

            foreach (var session in sessions2) {
                session.Receive(packet);
            }
            base.OnReceived(packet);
        }
    }
}
