using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;

namespace Dao.Net {

    public class InvocationContext {
        public InvocationContext(MethodBase method, object[] arguments) {
            Method = method;
            Arguments = arguments;
        }
        public MethodBase Method { get; private set; }
        public object[] Arguments { get; private set; }
        public object Return { get; set; }
    }

    public abstract class AbstractRealProxy : RealProxy {
        public AbstractRealProxy(Type type)
            : base(type) {

        }

        public abstract void OnInvoke(InvocationContext context);

        public override IMessage Invoke(IMessage msg) {
            var mcm = msg as IMethodCallMessage;
            var methodInfo = mcm.MethodBase as MethodInfo;
            try {
                //Console.WriteLine("Before");
                InvocationContext context = new InvocationContext(mcm.MethodBase, mcm.InArgs);
                OnInvoke(context);
                //Console.WriteLine("After");
                return new ReturnMessage(context.Return, null, 0, mcm.LogicalCallContext, mcm);
            } catch (Exception ex) {
                //Console.WriteLine("Exception");
                return new ReturnMessage(ex, mcm);
            }
        }
    }

    public class EventRealProxy : AbstractRealProxy {

        EventHandlerList handlers = new EventHandlerList();

        Dictionary<string, object> handlerKeys = new Dictionary<string, object>();

        public EventRealProxy(Type type) : base(type) {

        }

        protected object GetHandlerKey(string name) {
            object key;

            if (handlerKeys.TryGetValue(name, out key)) {
                return key;
            }

            key = new object();

            handlerKeys.Add(name, key);

            return key;
        }

        public override void OnInvoke(InvocationContext context) {

            Console.WriteLine(context.Method);

            MethodInfo method = context.Method as MethodInfo;

            if (method.IsSpecialName) {

                if (method.Name.StartsWith("add_")) {

                    string name = method.Name.Replace("add_", "");

                    object key = GetHandlerKey(name);

                    handlers.AddHandler(key, context.Arguments[0] as Delegate);

                    OnAddHandler(context, name, key);

                } else if (method.Name.StartsWith("remove_")) {


                    string name = method.Name.Replace("remove_", "");

                    object key = GetHandlerKey(name);

                    handlers.RemoveHandler(key, context.Arguments[0] as Delegate);

                    OnRemoveHandler(context, name, key);
                }
            } else {
                OnInvokeMethod(context);
            }
        }

        protected virtual void OnRemoveHandler(InvocationContext context,string name,object key) {

        }

        protected virtual void OnAddHandler(InvocationContext context, string name, object key) {

        }

        protected virtual void OnInvokeMethod(InvocationContext context) {

        }

        public void Raise(string name, params object[] args) {

            object key = GetHandlerKey(name);

            var handler = handlers[key];

            handler?.DynamicInvoke(args);
        }
    }

    public class AspectRealProxy<T> : AbstractRealProxy {
        public T Target { get; private set; }

        public AspectRealProxy(T target)
            : base(typeof(T)) {
            Target = target;
        }

        public override void OnInvoke(InvocationContext context) {
            context.Return = context.Method.Invoke(Target, context.Arguments);
        }

    }
}
