using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dao.Net {

    [Serializable]
    public class ServiceInvoke {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }
        public object[] Arguments { get; set; }
    }

    [Serializable]
    public class ServiceInvokeResult {
        public Guid Id { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public object ReturnValue { get; set; }
    }


    public class ServicePackets {
        public const int Invoke = 8001;
        public const int Reply = 8002;
    }

    public class ServiceManager : ISocketHandler {

        public void Handle(HandleContext context) {

            var packet = context.Packet;
            var session = context.Session;

            if (packet.Type == ServicePackets.Invoke) {

                ServiceInvoke info = packet.GetObject<ServiceInvoke>();

                Console.WriteLine("ServiceManager：收到调用{0} {1}", info.Name, info.Action);

                Packet reply = new Packet(ServicePackets.Reply);

                var result = Invoke(info);

                reply.SetObject(result);

                session.SendAsync(reply);
            }
        }

        Dictionary<string, object> services = new Dictionary<string, object>();

        public void AddService(string name, object service) {
            services.Add(name, service);
        }

        public List<object> GetServices() {
            return services.Values.ToList();
        }

        public object GetService(string name) {
            object service = null;
            services.TryGetValue(name, out service);
            return service;
        }

        protected virtual ServiceInvokeResult Invoke(ServiceInvoke info) {
            object service;

            ServiceInvokeResult result = new ServiceInvokeResult();

            result.Id = info.Id;

            if (services.TryGetValue(info.Name, out service)) {
                try {
                    MethodInfo method = service.GetType().GetMethod(info.Action);

                    if (method != null) {
                        try {
                            object ret = method.Invoke(service, info.Arguments);

                            result.Success = true;
                            result.ReturnValue = ret;
                        } catch(Exception) {
                            result.Message = "调用出错";
                        }
                    } else {
                        result.Message = "方法不存在";
                    }
                } catch {
                    result.Message = "查找方法出错";
                }

            } else {
                result.Message = "服务不存在";
            }
            return result;
        }
    }

}
