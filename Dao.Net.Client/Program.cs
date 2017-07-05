using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dao.Net.Client {
    class Program {
        static void Main(string[] args) {

            F();

            Console.ReadLine();

            //Application.EnableVisualStyles();
            //Form.CheckForIllegalCrossThreadCalls = false;
            //Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //Application.ThreadException += Application_ThreadException;
            //Application.Run(new frmUsers());
        }

        private async static void F() {

            Console.WriteLine("Client");
            MySocketClient client = new MySocketClient();

            await client.ConnectAsync("127.0.0.1", 1234);

        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e) {
            Console.WriteLine(e.Exception.Message);
        }
    }
}
