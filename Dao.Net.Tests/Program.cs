using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Dao.Dynamic;

namespace Dao.Net.Tests {
    class Program {
        static void Main(string[] args) {

            DynamicDelegate d = new DynamicDelegate();


            d.CreateDelegate(typeof(Action<string>), (e) => {
                Console.WriteLine(e.Length);
            });


            EventRealProxy proxy = new EventRealProxy(typeof(IHello));

            IHello hello = proxy.GetTransparentProxy() as IHello;

            hello.Hello += Hello_Hello;

            proxy.Raise("Hello", null, null);

            hello.Hello -= Hello_Hello;

            proxy.Raise("Hello", null, null);

            Console.WriteLine();
        }

        private static void Hello_Hello(object sender, EventArgs e) {
            Console.WriteLine("Hello");
        }
    }

    public interface IHello {
        event EventHandler Hello;
    }


}
