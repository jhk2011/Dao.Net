using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Dao.Net.Server {




    class MyServerSession : SocketSession {

        public MyServerSession(Socket socket, MySocketServer server) : base(socket) {

            Handlers.Add(new TransferHandler() { Server = server });

            ServiceClientHandler serviceClientHandler = new ServiceClientHandler() {
                Session = this
            };

            ServiceHandler serviceManager = new ServiceHandler();

            Handlers.Add(serviceManager);
            this.Handlers.Add(serviceClientHandler);
            Handlers.Add(new MyServerHandler());


            ICalcback calcCallback = serviceClientHandler.GetServiceProxy<ICalcback>("calcc");

            serviceManager.AddService("calc", new Calc() {
                callback = calcCallback
            });

            Calc2 calc2 = new Calc2();

            serviceManager.AddService("calc2", calc2);


            ITermainalCallbackService callback = serviceClientHandler
               .GetServiceProxy<ITermainalCallbackService>("tc");

            serviceManager.AddService("t", new TerminalService(callback));

            serviceManager.AddService("terminal", new TerminalService2());

        }
    }
}
