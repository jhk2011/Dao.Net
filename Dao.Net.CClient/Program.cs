using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dao.Net.CClient {
    static class Program {

        static void Main() {

            Console.WriteLine("CClient");

            //NewMethod();

            //Console.ReadLine();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            frmMain frm = new frmMain();

            Application.Run(frm);

        }

        private async static void NewMethod() {

            var client = new MySocketClient();

            await client.ConnectAsync("127.0.0.1", 1234);

            var result = client.calc.Add(100, 200);

            Console.WriteLine(result);
        }
    }
}
