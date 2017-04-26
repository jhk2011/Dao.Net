using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
        public string DestUserId { get; internal set; }
        public string ScrUserId { get; internal set; }
    }


    public class ServicePackets {
        public const int Invoke = 8001;
        public const int Reply = 8002;
    }

    public class SocketContext {

        static ThreadLocal<SocketContext> current = new ThreadLocal<SocketContext>();

        public static SocketContext Current
        {
            get
            {
                return current.Value;
            }

            internal set
            {
                current.Value = value;
            }
        }

        public Packet Packet { get; set; }

        public SocketSession Session { get; set; }

    }

    public class ServiceManager : ISocketHandler {

        public void Handle(Packet packet, SocketSession session) {

            if (packet.Type == ServicePackets.Invoke) {
                ServiceInvoke info = packet.GetObject<ServiceInvoke>();

                Console.WriteLine("ServiceManager：收到调用{0} {1}",info.Name,info.Action);

                Packet reply = new Packet(ServicePackets.Reply);

                UserManager userManager = session.Handlers.GetHandler<UserManager>();

                if (packet.DestUserId != null && userManager != null && packet.DestUserId != userManager.UserId) {

                    reply.DestUserId = packet.SrcUserId;
                    reply.SrcUserId = null;

                    var result = new ServiceInvokeResult {
                        Id = info.Id,
                        Success = false,
                        Message = "目标不在线"
                    };

                    reply.SetObject(result);

                    session.SendAsync(reply);

                } else {
                    var result = HandleInvoke(info);
                    result.Id = info.Id;

                    reply.DestUserId = packet.SrcUserId;
                    reply.SrcUserId = packet.DestUserId;

                    reply.SetObject(result);

                    session.SendAsync(reply);
                }
            } else if (packet.Type == ServicePackets.Reply) {

                Console.WriteLine("ServiceManager：收到调用回复");

                ServiceInvokeResult result = packet.GetObject<ServiceInvokeResult>();
                InvokeCompleted?.Invoke(result);
            }
        }

        public SocketSession Session { get; set; }

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

        public T GetServiceProxy<T>(string service, string userid = null)
            where T : class {
            ServiceProxy proxy = new ServiceProxy(typeof(T), this, service, userid);
            return proxy.GetTransparentProxy() as T;
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

        public void InvokeAsync(Guid id,
            string destUserId,
            string name,
            string action, params object[] args) {

            Console.WriteLine("ServiceManager:发起调用{0} {1}",name,action);

            ServiceInvoke info = new ServiceInvoke() {
                Name = name,
                Action = action,
                Arguments = args,
                Id = id
            };

            Packet p = new Packet(ServicePackets.Invoke);

            p.DestUserId = destUserId;
            p.SrcUserId = Session.Handlers.GetHandler<UserManager>()?.UserId;

            p.SetObject(info);
            Session.SendAsync(p);
        }

        public event Action<ServiceInvokeResult> InvokeCompleted;

        public Task<ServiceInvokeResult> InvokeTaskAsync(Guid id,
            string destUserId,
            string name,
            string action,
            params object[] args) {

            TaskCompletionSource<ServiceInvokeResult> tcs = new TaskCompletionSource<ServiceInvokeResult>();

            Action<ServiceInvokeResult> handler = null;

            handler = (info) => {
                if (info.Id == id) {
                    this.InvokeCompleted -= handler;
                    tcs.TrySetResult(info);
                }
            };

            Timer t = null;

            t = new Timer(_ => {
                this.InvokeCompleted -= handler;
                tcs.TrySetCanceled();
                t.Dispose();
            });

            t.Change(1000,Timeout.Infinite);

            this.InvokeCompleted += handler;

            this.InvokeAsync(id, destUserId, name, action, args);

            return tcs.Task;
        }
    }
}
