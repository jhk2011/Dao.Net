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
    public partial class frmTerminal : Form {
        public frmTerminal() {
            InitializeComponent();
        }

        internal MySocketClient Client { get; set; }

        public TerminalClientManager TerminalManager { get; set; }
        public string UserId { get; internal set; }

        internal void Init() {
            TerminalManager.Error += TerminalManager_Error;
            TerminalManager.Received += TerminalManager_Received;
            TerminalManager.InitAsync();
        }

        private void TerminalManager_Received(string obj) {
            textBox2.AppendText(obj) /*+ Environment.NewLine*/;
        }

        private void TerminalManager_Error(string obj) {
            textBox2.Text += obj + Environment.NewLine;
        }

        private void button1_Click(object sender, EventArgs e) {
            TerminalManager.ExecuteAsync(textBox1.Text + Environment.NewLine);
        }

        private void button2_Click(object sender, EventArgs e) {
            TerminalManager.CancelAsync();
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            TerminalManager.CloseAsync();
            TerminalManager.Error -= TerminalManager_Error;
            TerminalManager.Received -= TerminalManager_Received;
        }

        ClientSocketSession session;
        internal void Init(ClientSocketSession session) {
            this.session = session;
            TerminalManager = session.TerminalManager;
            Init();
        }
    }
}
