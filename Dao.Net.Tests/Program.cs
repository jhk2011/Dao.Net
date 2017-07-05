using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Dao.Net.Dynamic;

namespace Dao.Net.Tests
{
    class Program
    {
        static void Main(string[] args)
        {

            DynamicDelegate d = new DynamicDelegate();


            var de = d.CreateDelegate<Action<string, int>>((e) =>
               {
                   Console.WriteLine(e.Length);
               });

            de("abc",1);

            return;

            EventRealProxy proxy = new EventRealProxy(typeof(IHello));

            IHello hello = proxy.GetTransparentProxy() as IHello;

            hello.Hello += Hello_Hello;

            proxy.Raise("Hello", null, null);

            hello.Hello -= Hello_Hello;

            proxy.Raise("Hello", null, null);

            Console.WriteLine();
        }

        private static void Hello_Hello(object sender, EventArgs e)
        {
            Console.WriteLine("Hello");
        }
    }

    public interface IHello
    {
        event EventHandler Hello;
    }


}
