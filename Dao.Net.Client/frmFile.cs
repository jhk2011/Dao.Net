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
    public partial class frmFile : Form {
        public frmFile() {
            InitializeComponent();
        }

        IFileService fileService;

        internal void Init(MySocketClient client, string userid) {
            ServiceManager serviceManager = client.Handlers.GetHandler<ServiceManager>();
            fileService = serviceManager.GetServiceProxy<IFileService>("file", userid);
        }

        private void button1_Click(object sender, EventArgs e) {
            string path = textBox1.Text;
            Go(path);
        }

        private async void Go(string path) {
            listBox1.DisplayMember = "FullName";

            var files= await Task.Factory
                .StartNew(() => fileService.GetFileItem(path));

            if (files != null) {
                listBox1.DataSource = files;
                textBox1.Text = path;
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            DirectoryInfo di = new DirectoryInfo(textBox1.Text);
            if (di.Parent != null) {
                Go(di.Parent.FullName);
            } else {
                Go(null);
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e) {
            var f = listBox1.SelectedItem as FileItem;
            Go(f?.FullName);
        }
    }
}
