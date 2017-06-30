using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Dao.Net {

    public class ServiceProxy : BaseRealProxy {


        public string ServiceName { get; set; }

        public ServiceProxyManager ServiceProxyManager { get; set; }

        public ServiceProxy(Type type, ServiceProxyManager serviceProxyManager, string serviceName)
            : base(type) {

            ServiceProxyManager = serviceProxyManager;
            ServiceName = serviceName;
        }

        public override void OnInvoke(InvocationContext context) {

            var method = context.Method as MethodInfo;

            ServiceInvoke info = new ServiceInvoke {
                Name = ServiceName,
                Action = context.Method.Name,
                Arguments = context.Arguments,
                Id = Guid.NewGuid()
            };

            Packet p = new Packet(ServicePackets.Invoke);

            p.SetObject(info);

            context.Return = ServiceProxyManager.Invoke(info);

        }
    }

}
