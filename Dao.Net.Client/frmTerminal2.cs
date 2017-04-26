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
    public partial class frmTerminal2 : Form {
        public frmTerminal2() {
            Form.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        MySocketClient client;

        string userid;

        ITerminalService terminalServiceProxy;
        TerminalCallbackService terminalCallbackService;

        internal void Init(MySocketClient client, string userid) {
            this.client = client;
            this.userid = userid;
            var serviceManager = client.Handlers.GetHandler<ServiceManager>();

            terminalCallbackService = serviceManager
                .GetService("terminalCallback") as TerminalCallbackService;

            terminalCallbackService.Error += TerminalCallbackService_Error;
            terminalCallbackService.Received += TerminalCallbackService_Received;

            terminalServiceProxy = serviceManager
                .GetServiceProxy<ITerminalService>("terminal", userid);
        }

        private void TerminalCallbackService_Received(string obj) {

            SocketContext ctx = SocketContext.Current;

            if (ctx.Packet.SrcUserId != userid) return;

            textBox2.Text += obj;

        }

        private void TerminalCallbackService_Error(string obj) {
            SocketContext ctx = SocketContext.Current;

            if (ctx.Packet.SrcUserId != userid) return;

            textBox2.Text += obj;
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            terminalServiceProxy.Init();
        }

        private void button1_Click(object sender, EventArgs e) {
            terminalServiceProxy.Execute(textBox1.Text + "\r\n");
        }

        private void button2_Click(object sender, EventArgs e) {
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            terminalServiceProxy.Close();
        }

    }
}
