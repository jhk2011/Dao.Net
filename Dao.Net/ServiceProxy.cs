﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dao.Net {

    public class ServiceProxy : BaseRealProxy {

        public string ServiceName { get; set; }

        public ServiceClientHandler ServiceHandler { get; set; }

        public string UserId { get; set; }
        public ServiceProxy(Type type,
            ServiceClientHandler serviceHandler,
            string serviceName, string userId)
            : base(type) {

            ServiceHandler = serviceHandler;
            ServiceName = serviceName;
            UserId = userId;
        }

        public override void OnInvoke(InvocationContext context) {

            var method = context.Method as MethodInfo;

            ServiceInvoke info = new ServiceInvoke {
                Id = Guid.NewGuid(),
                Name = ServiceName,
                Action = context.Method.Name,
                Arguments = context.Arguments,
                DestUserId = UserId
            };

            Type returnType = method.ReturnType;

            if (typeof(Task).IsAssignableFrom(returnType)) {

                Type type = returnType.GetGenericArguments()[0];

                context.Return = ServiceHandler.GetType()
                    .GetMethod("InvokeTaskAsync2")
                    .MakeGenericMethod(type)
                    .Invoke(ServiceHandler, new object[] { info });

            } else {
                context.Return = ServiceHandler.Invoke(info);
            }
        }

    }

}
