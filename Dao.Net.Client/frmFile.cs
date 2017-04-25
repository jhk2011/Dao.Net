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
    public partial class frmFile : Form {
        public frmFile() {
            InitializeComponent();
            Start();
        }

        FileManager fileManager;
        UserManager userManager;
        private async void Start() {
            MySocketClient client = new MySocketClient();

            await client.ConnectAsync("127.0.0.1", 1234);

            fileManager = client.FileManager;
            userManager = client.UserManager;

            fileManager.GetFilesComplete += fileManager_ReceiveFiles;
            userManager.Join += UserManager_LoginCompleted;

            client.StartReceive();
        }

        private void UserManager_LoginCompleted(JoinReply obj) {
            Console.WriteLine(obj.Code == 0);
        }

        private void fileManager_ReceiveFiles(string[] obj) {
            listBox1.DataSource = obj;
        }

        private void button1_Click(object sender, EventArgs e) {
            fileManager.GetFilesAsync(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e) {

            userManager.JoinAsync("", "");
        }

        private void listBox1_DoubleClick(object sender, EventArgs e) {
            string path = listBox1.SelectedItem as string;
            textBox1.Text = path;
            fileManager.GetFilesAsync(path);
        }
    }
}
