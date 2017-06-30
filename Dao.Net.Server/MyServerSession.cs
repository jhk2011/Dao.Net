using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Dao.Net.Server {




    class MyServerSession : SocketSession {

        public MySocketServer Server { get; set; }

        public MyServerSession(Socket socket, MySocketServer server) : base(socket) {

            Server = server;

            this.Handlers = new SocketHandlerCollection();

            ServiceProxyManager proxyManager = new ServiceProxyManager() {
                Session = this
            };

            ServiceManager serviceManager = new ServiceManager();

            ICalcTip tip = proxyManager.GetServiceProxy<ICalcTip>("calct");

            this.Handlers.Add(proxyManager);

            serviceManager.AddService("calc", new Calc() {
                tip = tip
            });

            ITermainalCallbackService callback = proxyManager.GetServiceProxy<ITermainalCallbackService>("tc");

            serviceManager.AddService("t", new TerminalService(callback));

            Handlers.Add(serviceManager);
        }

        protected override void OnReceived(Packet packet) {
            base.OnReceived(packet);
            //System.Console.WriteLine("OnReceived:{0}", packet.Type);
        }
    }
}
