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

            //Terminal t = new Terminal();

           
            //t.Received += T_Received;
            //t.Init();
            //t.Close();
            //t.Init();

            //while (true) {
            //    t.Execute(Console.ReadLine()+"\r\n");
            //}

            Application.EnableVisualStyles();
            Application.Run(new frmMain());
        }

        private static void T_Received(string obj) {
            Console.Write(obj);
        }

        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
    }
}
