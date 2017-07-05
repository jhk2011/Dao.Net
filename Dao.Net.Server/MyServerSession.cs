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

            ICalcback calcCallback = serviceClientHandler.GetServiceProxy<ICalcback>("calcc");

            this.Handlers.Add(serviceClientHandler);

            serviceManager.AddService("calc", new Calc() {
                callback = calcCallback
            });

            ITermainalCallbackService callback = serviceClientHandler
                .GetServiceProxy<ITermainalCallbackService>("tc");

            serviceManager.AddService("t", new TerminalService(callback));

            Handlers.Add(serviceManager);

            Handlers.Add(new MyServerHandler());
        }

    }
}
