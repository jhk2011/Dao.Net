using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dao.Net.CClient {
    static class Program {

        static void Main() {
           
            //Stopwatch w = Stopwatch.StartNew();

            //var result = client.calc.Add(100, 200);

            //w.Stop();

            //Console.WriteLine("ElapsedMilliseconds:{0}", w.ElapsedMilliseconds);

            //Console.WriteLine(result);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            frmMain frm = new frmMain();

            Application.Run(frm);

        }

    }
}
