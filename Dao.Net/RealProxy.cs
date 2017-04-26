using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;

namespace Dao.Net
{

    public class InvocationContext
    {
        public InvocationContext(MethodBase method, object[] arguments)
        {
            Method = method;
            Arguments = arguments;
        }
        public MethodBase Method { get; private set; }
        public object[] Arguments { get; private set; }
        public object Return { get; set; }
    }

    public abstract class BaseRealProxy : RealProxy
    {
        public BaseRealProxy(Type type)
            : base(type)
        {

        }

        public abstract void OnInvoke(InvocationContext context);

        public override IMessage Invoke(IMessage msg)
        {
            var mcm = msg as IMethodCallMessage;
            var methodInfo = mcm.MethodBase as MethodInfo;
            try
            {
                //Console.WriteLine("Before");
                InvocationContext context = new InvocationContext(mcm.MethodBase, mcm.InArgs);
                OnInvoke(context);
                //Console.WriteLine("After");
                return new ReturnMessage(context.Return, null, 0, mcm.LogicalCallContext, mcm);
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Exception");
                return new ReturnMessage(ex, mcm);
            }
        }
    }

    public class AspectRealProxy<T> : BaseRealProxy
    {
        public T Target { get; private set; }

        public AspectRealProxy(T target)
            : base(typeof(T))
        {
            Target = target;
        }

        public override void OnInvoke(InvocationContext context)
        {
            context.Return = context.Method.Invoke(Target, context.Arguments);
        }

    }
}
