using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dao.Net.CClient {


    class MySocketClient : TaskSocketClient {


        public MySocketClient() {
            this.Handlers = new PacketHandlerCollection();


            var UserManager = new UserManager() { Session = this };

            var TerminalManager = new TerminalClientManager { Session = this };

            this.Handlers.Add(UserManager);
            this.Handlers.Add(TerminalManager);

            this.Handlers.Add(new TerminalServerManager() {
                Session = this
            });

            var serviceManager = new ServiceManager {
                Session = this
            };

            serviceManager.AddService("calc", new Calc());

            serviceManager.AddService("terminal", new TerminalService());

            this.Handlers.Add(serviceManager);

        }

        protected override void OnReceived(Packet packet) {
            //Console.WriteLine("Received Type={0}", packet.Type);
            base.OnReceived(packet);
        }
    }
}
