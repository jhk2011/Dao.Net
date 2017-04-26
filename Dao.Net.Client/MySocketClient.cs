using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dao.Net.Client {


    class MySocketClient : TaskSocketClient {

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

            var serviceManager = new ServiceManager {
                Session = this
            };

            serviceManager.AddService("calcCallback", new CalcCallback());

            serviceManager.AddService("terminalCallback", new TerminalCallbackService());

            this.Handlers.Add(serviceManager);

        }



        protected override void OnReceived(Packet packet) {
            base.OnReceived(packet);
        }
    }
}
