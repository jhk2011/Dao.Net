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

        ServiceManager serviceHandler;

        internal void Init(MySocketClient client) {
            serviceHandler = client.Handlers.GetHandler<ServiceManager>();
        }

        private async void button1_Click(object sender, EventArgs e) {
            var result = await serviceHandler
                .InvokeTaskAsync(Guid.NewGuid(), "calc", "Add", 1, 2);

            var result2 = await serviceHandler
                .InvokeTaskAsync(Guid.NewGuid(), "calc", "Add",4, 6);

            if (result.Success) {
                Console.WriteLine(result.ReturnValue);
            } else {
                Console.WriteLine(result.Message);
            }
        }
    }
}
