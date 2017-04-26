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

        MySocketClient client;
        public async void Start() {

            if (client != null) client.Socket.Close();

            client = new MySocketClient();

            string host = textBox2.Text;

            if (string.IsNullOrEmpty(host)) {
                host = "127.0.0.1";
            }
            await client.ConnectAsync(host, 1234);

            client.StartReceive();

            var userManager = client.Handlers.GetHandler<UserManager>();

            userManager.JoinAsync("test", "");
            Console.WriteLine("CClient");
        }

        private void button5_Click(object sender, EventArgs e) {
            Start();
        }
    }
}
