using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Net {

    public interface IService {
        void OnClose();
        void OnAccept();
    }

    [Serializable]
    public class ServiceInvoke : ITransferable {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }
        public object[] Arguments { get; set; }

        public string SrcUserId { get; set; }

        public string DestUserId { get; set; }

        public string Instance { get; set; }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();

            if (SrcUserId != null) {
                sb.AppendLine("ScrUserId:" + SrcUserId);
            }
            if (DestUserId != null) {
                sb.AppendLine("DestUserId:" + DestUserId);
            }

            sb.AppendLine(string.Format("{0}.{1}(2)", Name, Action, Instance ?? "0"));


            return sb.ToString();
        }
    }

    [Serializable]
    public class EventInfo : ITransferable {
        public Guid Id { get; set; }
        public string SrcUserId { get; set; }

        public string DestUserId { get; set; }

        public string Name { get; set; }

        public string Event { get; set; }

        public object[] Arguemnts { get; set; }

        public string Instance { get; set; }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();

            if (SrcUserId != null) {
                sb.AppendLine("ScrUserId:" + SrcUserId);
            }
            if (DestUserId != null) {
                sb.AppendLine("DestUserId:" + DestUserId);
            }

            sb.AppendLine(string.Format("{0}.{1}(2)", Name, Event, Instance ?? "0"));


            return sb.ToString();
        }

    }

    [Serializable]
    public class Subscribe : ITransferable {
        public Guid Id { get; set; }

        public string SrcUserId { get; set; }

        public string DestUserId { get; set; }

        public string Name { get; set; }

        public string Action { get; set; }

        public string Instance { get; set; }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();

            if (SrcUserId != null) {
                sb.AppendLine("ScrUserId:" + SrcUserId);
            }
            if (DestUserId != null) {
                sb.AppendLine("DestUserId:" + DestUserId);
            }

            sb.AppendLine(string.Format("{0}.{1}(2)", Name, Action, Instance));


            return sb.ToString();
        }

    }

    [Serializable]
    public class ServiceInvokeResult : ITransferable, IResponse {
        public Guid Id { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }

        public string SrcUserId { get; set; }

        public string DestUserId { get; set; }

        public object ReturnValue { get; set; }

        public string Instance { get; set; }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();

            if (SrcUserId != null) {
                sb.AppendLine("ScrUserId:" + SrcUserId);
            }
            if (DestUserId != null) {
                sb.AppendLine("DestUserId:" + DestUserId);
            }

            sb.AppendLine(string.Format("{0}(2)", ReturnValue, Instance));

            return sb.ToString();
        }

    }

    public class ServiceDef {
        public Func<object> ServiceLoader { get; set; }
        public Dictionary<string, ServiceInstance> Instances { get; set; }
        public string Name { get; internal set; }
    }

    public class ServiceInstance {
        public string Id { get; set; }
        public object Service { get; set; }
    }

    public class ServiceHandler : SocketHandler {

        public override void Close(HandleContext context) {
            foreach (var service in services) {
                IService iservice = service.Value as IService;
                iservice?.OnClose();
            }
        }

        SocketSession session = null;

        public override void Accept(HandleContext context) {

            session = context.Session;

            foreach (var service in services) {
                IService iservice = service.Value as IService;
                iservice?.OnAccept();
            }
        }

        public override async void Handle(HandleContext context) {

            var packet = context.Packet;
            var session = context.Session;

            ServiceInvoke info = packet as ServiceInvoke;

            if (info != null) {

                context.Cancel = true;

                Console.WriteLine("收到调用{0}.{1} {2}->{3}",
                    info.Name, info.Action, info.SrcUserId, info.DestUserId);

                var result = Invoke(info);

                await session.SendAsync(result);
            } else {
                Subscribe sub = packet as Subscribe;

                if (sub != null) {

                    context.Cancel = true;

                    Console.WriteLine("收到事件注册");

                    object service = GetService(sub.Name, sub.Instance);

                    var e = service.GetType().GetEvent(sub.Action);

                    //AddEventHandler(sub.Name, service);
                }
            }
        }

        Dictionary<string, object> services = new Dictionary<string, object>();

        Dictionary<string, ServiceDef> serviceDefs = new Dictionary<string, ServiceDef>();

        public void AddService(string name, object service) {

            ServiceDef def = new ServiceDef {
                Instances = new Dictionary<string, ServiceInstance> {
                    [""] = new ServiceInstance {
                        Id = "",
                        Service = service
                    }
                },
                Name = name
            };

            serviceDefs.Add(name, def);

            //services.Add(name, service);
            AddEventHandler(name, service, "");
        }

        private void AddEventHandler(string name, object service, string instance) {
            foreach (var e in service.GetType().GetEvents()) {
                var method = e.GetAddMethod();

                var handler = GetDelegate(e.EventHandlerType, (args) => {
                    Raise(session, name, e.Name, instance, args);
                });
                method.Invoke(service, new object[] { handler });
            }
        }

        public void AddService(string name, Func<object> serviceLoader) {
            ServiceDef def = new ServiceDef {
                Name = name,
                ServiceLoader = serviceLoader,
                Instances = new Dictionary<string, ServiceInstance>()
            };
            serviceDefs.Add(name, def);
        }
        public object GetService(string name, string instanceId = null) {

            if (string.IsNullOrEmpty(instanceId)) {
                instanceId = "";
            }

            object service = null;


            if (services.TryGetValue(name, out service)) {
                return service;
            }

            ServiceDef def = null;

            if (serviceDefs.TryGetValue(name, out def)) {
                ServiceInstance instance;
                if (def.Instances.TryGetValue(instanceId, out instance)) {
                    service = instance.Service;
                } else {

                    if (def.ServiceLoader == null) {
                        return null;
                    }

                    service = def.ServiceLoader();

                    instance = new ServiceInstance {
                        Service = service
                    };
                    def.Instances.Add(instanceId, instance);
                    AddEventHandler(name, service, instanceId);
                }
                return instance.Service;
            }
            return null;
        }

        public async void Raise(SocketSession session, string name, string e, string instance, params object[] args) {

            Console.WriteLine("触发事件");

            await session.SendAsync(new EventInfo {
                Event = e,
                Name = name,
                Id = Guid.NewGuid(),
                Arguemnts = args,
                Instance = instance
            });
        }


        public Delegate GetDelegate(Type type, Action<object[]> handler) {

            Dao.Net.Dynamic.DynamicDelegateBuilder b = new Dynamic.DynamicDelegateBuilder();

            return b.CreateDelegate(type, handler);

        }


        protected virtual ServiceInvokeResult Invoke(ServiceInvoke info) {

            ServiceInvokeResult result = new ServiceInvokeResult();

            result.Id = info.Id;
            result.DestUserId = info.SrcUserId;
            result.SrcUserId = info.DestUserId;
            result.Instance = info.Instance;

            object service = GetService(info.Name, info.Instance);

            if (service == null) {
                result.Message = "服务不存在";
                return result;
            }

            MethodInfo method = null;

            try {
                method = service.GetType().GetMethod(info.Action);

                if (method == null) {
                    result.Message = "方法不存在";
                    return result;
                }

            } catch {
                result.Message = "查找方法出错";
                return result;
            }

            object ret;

            try {
                ret = method.Invoke(service, info.Arguments);
            } catch (Exception ex) {
                ex = ex.InnerException;
                result.Message = "调用出错";
                return result;
            }

            Task task = ret as Task;

            if (task != null) {
                if (task.IsCompleted) {
                    ret = task.GetType().GetProperty("Result").GetValue(task);
                }
                //TODO 等待完成
            }

            result.Success = true;
            result.ReturnValue = ret;

            return result;
        }
    }

}
