using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dao.Net.Client {


    class MySocketClient : SocketClient {


        public MySocketClient() {

            ServiceHandler serviceHandler = new ServiceHandler();

            this.Handlers.Add(new SocketClientHandler());

            serviceHandler.AddService("calc", new Calc());

            this.Handlers.Add(serviceHandler);

        }
    }
}
