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
    public partial class frmMain : Form {
        private MySocketClient client;

        public frmMain() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            var form = new frmTerminal();
            form.Init(client);
            form.Show();
        }

        private void Form2_Load(object sender, EventArgs e) {

        }

        private async void button2_Click(object sender, EventArgs e) {

            client = new MySocketClient();

            await client.ConnectAsync(textBox1.Text, 1234)
               /* .ConfigureAwait(false)*/;

            client.StartReceive();

            client.Closed += Client_Closed;

            button2.Enabled = false;
        }

        private void Client_Closed(object sender, EventArgs e) {
            button2.Enabled = true;
        }
    }
}
