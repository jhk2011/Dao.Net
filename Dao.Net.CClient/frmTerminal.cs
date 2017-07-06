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
    public partial class frmTerminal : Form {

        MySocketClient client;


        ITerminalServcie3 terminal;


        public frmTerminal() {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            terminal.Init();
        }

        internal void Init(MySocketClient client) {
            this.client = client;

            terminal = client.serviceClientHandler
                .GetServiceProxy<ITerminalServcie3>("terminal", "0","");

            terminal.Received += Terminal2_Received;
            terminal.Error += Terminal2_Error;
            terminal.Closed += Terminal_Closed;

            //terminal = client.Terminal;

            //client.TerminalCallback.Receive += Terminal2_Received;

            //client.TerminalCallback.Error += Terminal2_Error;
        }

        private void Terminal_Closed() {
                MessageBox.Show("关闭");
        }

        private void Terminal2_Error(string ss) {

            textBox2.Text += ss + "\r\n";

            textBox2.Select(textBox2.Text.Length - 1, 0);
            textBox2.ScrollToCaret();
        }

        private void Terminal2_Received(string ss) {

            textBox2.Text += ss + "\r\n";

            textBox2.Select(textBox2.Text.Length - 1, 0);
            textBox2.ScrollToCaret();
        }

        private void button1_Click(object sender, EventArgs e) {
            Execute();
        }

        private void Execute() {
            terminal.Execute(textBox1.Text + "\r\n" + "\r\n");

            textBox1.Text = "";
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            terminal.Close();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                Execute();
            }
        }
    }
}
