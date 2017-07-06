using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Dao.Net.Client {


    class MySocketClient : SyncSocketClient {


        public MySocketClient() {

            //ServiceHandler serviceHandler = new ServiceHandler();

            //this.Handlers.Add(new SocketClientHandler());

            //serviceHandler.AddService("calc", new Calc());


            //serviceHandler.AddService("terminal", new TerminalService2());

            //this.Handlers.Add(serviceHandler);

            this.Handlers.Add(new MyTransferHandler() {

            });

        }
    }

    class MyTransferHandler : TransferClientHandler {
        public MyTransferHandler() {

        }

        protected override SocketSession GetSession(Socket socket, string userId) {
            return new MyTransferSession(socket, userId);
        }
    }

    class MyTransferSession : TransferSession {
        public MyTransferSession(Socket socket, string userid) : base(socket, userid) {

            ServiceHandler serviceHandler = new ServiceHandler();

            this.Handlers.Add(new SocketClientHandler());

            serviceHandler.AddService("calc", new Calc());

            serviceHandler.AddService("calc2", new Calc2());

            serviceHandler.AddService("terminal", new TerminalService2());

            this.Handlers.Add(serviceHandler);

        }
    }
}
