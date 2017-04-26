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
    public partial class frmService : Form {
        public frmService() {
            InitializeComponent();
        }

        string userId;

        ServiceManager serviceHandler;

        private async void button1_Click(object sender, EventArgs e) {

            ICalc calc = serviceHandler.GetServiceProxy<ICalc>("calc", userId);

            var result = await Task.Factory.StartNew(() => {
                return calc.Add(1, 2);
            });

            label1.Text = result + "";

            //var result = await serviceHandler
            //    .InvokeTaskAsync(Guid.NewGuid(), user, "calc", "Add", 1, 2);

            //var result2 = await serviceHandler
            //    .InvokeTaskAsync(Guid.NewGuid(), user, "calc", "Add", 4, 6);

            //if (result.Success) {
            //    label1.Text = result.ReturnValue + "";
            //} else {
            //    label1.Text = result.Message;
            //}
        }

        internal void Init(string userId, ServiceManager servicemanager) {
            this.userId = userId;
            this.serviceHandler = servicemanager;
            var callback = this.serviceHandler.GetService("calcCallback") as CalcCallback;
            callback.Info += Callback_Info;
            label1.Text = userId;
        }

        private void Callback_Info(string obj) {
            Console.WriteLine(obj);
        }
    }
}
