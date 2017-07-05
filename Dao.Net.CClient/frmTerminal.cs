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

        ITerminalService terminal;

        int id;

        public frmTerminal() {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            id = terminal.Init();
        }

        internal void Init(MySocketClient client) {
            this.client = client;

            terminal = client.Terminal;

            client.TerminalCallback.Receive += (idd, ss) => {

                if (idd != id) return;

                textBox2.Text += ss + "\r\n";

                textBox2.Select(textBox2.Text.Length - 1, 0);
                textBox2.ScrollToCaret();
            };

            client.TerminalCallback.Error += (idd, ss) => {

                if (idd != id) return;

                textBox2.Text += ss + "\r\n";

                textBox2.Select(textBox2.Text.Length - 1, 0);
                textBox2.ScrollToCaret();
            };
        }

        private void button1_Click(object sender, EventArgs e) {

            terminal.Execute(id, textBox1.Text + "\r\n");

            textBox1.Text = "";
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            terminal.Close(id);
        }
    }
}
