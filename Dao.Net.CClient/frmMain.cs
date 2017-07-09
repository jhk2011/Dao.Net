using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dao.Net.CClient {
    public partial class frmMain : Form {

        private MySocketClient client;

        public frmMain() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            var form = new frmTerminal();
            form.Init(client);
            form.Show();
        }

        private void Form2_Load(object sender, EventArgs e) {

        }

        private async void button2_Click(object sender, EventArgs e) {

            client = new MySocketClient();

            client.Closed += Client_Closed;

            await client.ConnectAsync(textBox1.Text, 1234);

            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);

            button2.Enabled = false;
        }

        private void Client_Closed(object sender, EventArgs e) {
            button2.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e) {

            ICalc calc = client.serviceClientHandler
                .GetServiceProxy<ICalc>("calc", textBox2.Text);

            var result = calc.Add(1, 2);

            Console.WriteLine(result);

            //client.SendAsync("test");
            //client.SendAsync(DateTime.Now);
            //client.SendAsync(Guid.NewGuid());
        }

        private void button4_Click(object sender, EventArgs e) {
            var result = client.calc.Add(100, 200);
            Console.WriteLine(result);
        }

        private async void button5_Click(object sender, EventArgs e) {
            var result = await client.calc.AddAsync(100, 200);
            Console.WriteLine(result);
        }

        private void button6_Click(object sender, EventArgs e) {

            ICalc2 calc2 = client.serviceClientHandler.GetServiceProxy<ICalc2>("calc2");

            calc2.Added += Calc2_Added;

            var result = calc2.Add(100, 200);

            Console.WriteLine(result);

            calc2.Added -= Calc2_Added;

        }

        private void Calc2_Added(string s) {
            Console.WriteLine("Calc2.Added:" + s);
        }

        private void button7_Click(object sender, EventArgs e) {

            ICalc2 calc2 = client.serviceClientHandler.GetServiceProxy<ICalc2>("calc2", "0");

            calc2.Added += Calc2_Added;

            var result = calc2.Add(100, 200);

            Console.WriteLine(result);

            calc2.Added -= Calc2_Added;
        }

        private async void button8_Click(object sender, EventArgs e) {
            while (true) {
                await client.SendAsync(new byte[1024000]);
            }
        }
    }
}
