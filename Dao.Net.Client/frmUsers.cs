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

        private MySocketClient _client;
        private UserManager userManager;

        public frmUsers() {
            InitializeComponent();
        }

        internal void Init(MySocketClient client) {
            _client = client;
            userManager = _client.UserManager;
            userManager.GetUsersCompleted += UserManager_GetUsersCompleted;
        }

        private void UserManager_GetUsersCompleted(List<string> obj) {
            listBox1.DataSource = obj;
        }

        private void button1_Click(object sender, EventArgs e) {
            userManager.LoginAsync(textBox1.Text, "");
        }

        private void button2_Click(object sender, EventArgs e) {
            userManager.GetUserAsync();
        }
    }
}
