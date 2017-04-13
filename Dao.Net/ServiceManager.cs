using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        public void Handle(Packet packet, SocketSession session) {
            if (packet.Type == ServicePackets.Invoke) {
                ServiceInvoke info = packet.GetObject<ServiceInvoke>();
                var result = HandleInvoke(info);
                Packet p = new Packet(ServicePackets.Reply);
                result.Id = info.Id;
                p.SetObject(result);
                session.SendAsync(p);
            } else if (packet.Type == ServicePackets.Reply) {
                ServiceInvokeResult result = packet.GetObject<ServiceInvokeResult>();
                InvokeCompleted?.Invoke(result);
            }
        }

        public SocketSession Session { get; set; }

        Dictionary<string, object> services = new Dictionary<string, object>();

        public void AddService(string name, object service) {
            services.Add(name, service);
        }

        private ServiceInvokeResult HandleInvoke(ServiceInvoke info) {
            object service;
            ServiceInvokeResult result = new ServiceInvokeResult();
            if (services.TryGetValue(info.Name, out service)) {
                try {
                    MethodInfo method = service.GetType().GetMethod(info.Action);

                    if (method != null) {
                        try {
                            object ret = method.Invoke(service, info.Arguments);
                            result.Success = true;
                            result.ReturnValue = ret;
                        } catch {
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

        public void InvokeAsync(Guid id,string name, string action, params object[] args) {
            ServiceInvoke info = new ServiceInvoke() {
                Name = name,
                Action = action,
                Arguments = args,
                Id = id
            };
            Packet p = new Packet(ServicePackets.Invoke);
            p.SetObject(info);
            Session.SendAsync(p);
        }

        public event Action<ServiceInvokeResult> InvokeCompleted;

        public Task<ServiceInvokeResult> InvokeTaskAsync(Guid id,string name, string action, params object[] args) {

            TaskCompletionSource<ServiceInvokeResult> tcs = new TaskCompletionSource<ServiceInvokeResult>();

            Action<ServiceInvokeResult> handler = null;

            handler = (info) => {
                if (info.Id == id) {
                    this.InvokeCompleted -= handler;
                    tcs.TrySetResult(info);
                }
            };

            this.InvokeCompleted += handler;

            this.InvokeAsync(id, name, action, args);

            return tcs.Task;
        }
    }
}
