using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dao.Net.Client {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        MySocketClient client;
        private async void button1_Click(object sender, EventArgs e) {

            Console.WriteLine("Client");
            client = new MySocketClient();
            client.Closed += (ss, ee) => {
                button1.Enabled = true;
            };
            try {
                await client.ConnectAsync(textBox1.Text, 1234);
                button1.Enabled = false;
            } catch {
                Console.WriteLine("连接失败");
            }
        }
    }
}
