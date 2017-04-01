using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Net.Server {
    partial class Program {
        static void Main(string[] args) {

            MySocketServer server = new MySocketServer(new MySocketListener());
            server.Initialize(1234);
            Console.ReadLine();
        }
    }
}
