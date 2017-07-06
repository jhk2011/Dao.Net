using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dao.Net.Dynamic
{
    public class DynamicDelegate
    {


        public Delegate CreateDelegate(Type type, Action<object[]> action)
        {
            DynamicDelegateBuilder builder = new DynamicDelegateBuilder();
            return builder.CreateDelegate(type, action);
        }

        public T CreateDelegate<T>(Action<object[]> action)
            where T : class
        {
            return CreateDelegate(typeof(T), action) as T;
        }
    }


    public class DynamicDelegateDemo
    {

        public DynamicDelegateDemo(Action<object[]> handler)
        {
            this.handler = handler;
        }

        Action<object[]> handler;

        public void Invoke(string s, int a)
        {
            handler.Invoke(new object[] { s, a });
        }
    }
}
