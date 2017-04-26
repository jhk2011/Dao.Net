using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Dao.Net {

    public class ServiceProxy : BaseRealProxy {
        public string ServiceName { get; set; }
        public ServiceManager ServiceManager { get; set; }

        public string UserId { get; set; }

        public ServiceProxy(Type type, ServiceManager serviceManager, string serviceName,string userId)
            : base(type) {
            ServiceManager = serviceManager;
            ServiceName = serviceName;
            UserId = userId;
        }

        public override void OnInvoke(InvocationContext context) {

            var result = ServiceManager.InvokeTaskAsync(Guid.NewGuid(), UserId,
                ServiceName, context.Method.Name,
                context.Arguments).Result;

            if (result.Success) {
                context.Return = result.ReturnValue;
            } else {
                throw new InvalidOperationException(result.Message);
            }

        }
    }

}
