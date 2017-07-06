using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dao.Net {

    public class ServiceProxy : EventRealProxy {
        public ServiceClientHandler ServiceClientHandler { get; set; }

        public string ServiceName { get; set; }

        public string Instance { get; set; }

        public string UserId { get; set; }
        public ServiceProxy(Type type,
            ServiceClientHandler serviceHandler,
            string serviceName, string userId, string instance = null)
            : base(type) {

            ServiceClientHandler = serviceHandler;
            ServiceName = serviceName;
            UserId = userId;
            Instance = instance;

            ServiceClientHandler.Raised += ServiceHandler_Raised;
        }

        private void ServiceHandler_Raised(EventInfo obj) {
            if (obj.Name == ServiceName && obj.Instance == Instance) {
                Raise(obj.Event, obj.Arguemnts);
            }
        }

        protected override void OnAddHandler(InvocationContext context, string name, object key) {
            //ServiceClientHandler.Subscribe(new Subscribe {
            //    Id = Guid.NewGuid(),
            //    Name = ServiceName,
            //    Action = name,
            //    DestUserId = UserId,
            //    Instance = Instance
            //});
        }

        protected override void OnRemoveHandler(InvocationContext context, string name, object key) {

        }

        protected override void OnInvokeMethod(InvocationContext context) {

            var method = context.Method as MethodInfo;

            ServiceInvoke info = new ServiceInvoke {
                Id = Guid.NewGuid(),
                Name = ServiceName,
                Action = context.Method.Name,
                Arguments = context.Arguments,
                Instance = Instance,
                DestUserId = UserId,
            };

            Type returnType = method.ReturnType;

            if (typeof(Task).IsAssignableFrom(returnType)) {

                Type type = returnType.GetGenericArguments()[0];

                context.Return = ServiceClientHandler.GetType()
                    .GetMethod("InvokeTaskAsync2")
                    .MakeGenericMethod(type)
                    .Invoke(ServiceClientHandler, new object[] { info });

            } else {
                context.Return = ServiceClientHandler.Invoke(info);
            }
        }

    }

}
