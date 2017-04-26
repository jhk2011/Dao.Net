using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dao.Net.Client {
    public partial class frmMain : Form {
        public frmMain() {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            try {
                textBox1.Text = File.ReadAllText("ip.txt");
            } catch {
                
            }
            Start();
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            File.WriteAllText("ip.txt", textBox1.Text);
        }

        MySocketClient client = new MySocketClient();

        private async void Start() {
            try {
                client = new MySocketClient();
                await client.ConnectAsync(textBox1.Text, 1234);
                client.StartReceive();
                Console.WriteLine("OK");
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            Start();
        }

        private void button2_Click(object sender, EventArgs e) {

        }

        private void button3_Click(object sender, EventArgs e) {
            
        }

        private void button4_Click(object sender, EventArgs e) {

        }
    }
}
