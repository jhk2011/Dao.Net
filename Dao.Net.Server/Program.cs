using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dao.Net.Server {
    partial class Program {

        static void Main(string[] args) {

            HttpSocketServier httpServer = new HttpSocketServier();

            httpServer.Initialize(8080);

            MySocketServer server = new MySocketServer(new MySocketListener());

            server.Initialize(1234);


            //Form1 form = new Form1();

            //form.Load += (s, e) => {
            //    F();

            //    Print("after f");
            //};

            //Application.EnableVisualStyles();
            //Application.Run(form);


            Console.ReadLine();
        }

        static Task TestTask() {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            new Thread(() => {
                tcs.TrySetResult(null);
                Print("set result");
            }).Start();

            return tcs.Task;
        }


        static async void F() {

            Print("before await");

            await TestTask().ConfigureAwait(false);

            Print("after await");
        }

        static void Print(string s = null) {
            Console.WriteLine(s + ":ThreadId:{0}", Thread.CurrentThread.ManagedThreadId);
        }

    }
}
