using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dao.Net.CClient {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
            Start();
        }

        public async void Start() {
            MySocketClient client = new MySocketClient();

            await client.ConnectAsync("127.0.0.1", 1234);

            client.StartReceive();

            var userManager = client.Handlers.GetHandler<UserManager>();

            userManager.JoinAsync("test", "");
            Console.WriteLine("CClient");
        }
    }
}
