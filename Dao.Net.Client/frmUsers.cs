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
    public partial class frmUsers : Form {

        private MySocketClient client;
        private UserManager userManager;

        public frmUsers() {
            InitializeComponent();
            Start();
        }

        private async void Start() {
            try {
                client = new MySocketClient();

                string host = textBox2.Text;

                if (string.IsNullOrEmpty(host)) {
                    host = "127.0.0.1";
                }

                await client.ConnectAsync(host, 1234);
                client.StartReceive();

                userManager = client.Handlers.GetHandler<UserManager>();

                userManager.Join += UserManager_Join;
                userManager.GetUsers += UserManager_GetUsers;


                Console.WriteLine("OK");
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        private void UserManager_GetUsers(List<string> obj) {
            listBox1.DataSource = obj;
        }

        private void button1_Click(object sender, EventArgs e) {
            userManager.JoinAsync(textBox1.Text, "");
        }

        private void UserManager_Join(JoinReply obj) {
            if (obj.Code == 0) {
                Console.WriteLine("Join success");
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            userManager.GetUserAsync();
        }

        private void button3_Click(object sender, EventArgs e) {
            frmTerminal frm = new frmTerminal();
            string user = listBox1.SelectedItem as string;
            ClientSocketSession session = client.GetSession(user);
            frm.Init(session);
            frm.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e) {
            frmService frm = new frmService();
            string user = listBox1.SelectedItem as string;

            frm.Init(user, client.Handlers.GetHandler<ServiceManager>());

            frm.ShowDialog();

        }

        private void button5_Click(object sender, EventArgs e) {
            Start();
        }

        private void button6_Click(object sender, EventArgs e) {
            frmTerminal2 frm = new frmTerminal2();
            string user = listBox1.SelectedItem as string;
            frm.Init(client, user);
            frm.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e) {
            //Packet p = new Packet(99999);
            //p.SrcUserId = "abc";
            //p.DestUserId = "test";
            //client.SendAsync(p);
        }
    }
}
