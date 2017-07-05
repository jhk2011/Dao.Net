using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dao.Dynamic {
    public class DynamicDelegate {
        public Delegate CreateDelegate(Type type, Action<object[]> action) {
            DynamicDelegateBuilder builder = new DynamicDelegateBuilder();
            return builder.CreateDelegate(type, action);
        }
    }
    public class MyClass {
        Action<object[]> handler;

        void Invoke(string s, int a) {
            handler.Invoke(new object[] { s, a });
        }
    }
}
