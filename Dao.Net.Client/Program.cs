using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dao.Net.Client {
    class Program {
        static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.Run(new Form1());
            return;
            for (int i = 0; i < 1; i++) {
                Start();
            }
            Console.ReadLine();
        }

        private static async void Start() {
            SocketClient client = new MySocketClient();

            FileManager fileManager = new FileManager(client);

            client.Handlers.Add(fileManager);

            fileManager.GetFilesComplete += fileManager_ReceiveFiles;

            await client.ConnectAsync("127.0.0.1", 1234);
            client.StartReceive();

            fileManager.GetFilesAsync(@"C:/");

        }

        private static void fileManager_ReceiveFiles(string[] obj) {
            Console.WriteLine(obj);
        }
    }
}
