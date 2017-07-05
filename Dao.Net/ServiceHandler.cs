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
    }

    [Serializable]
    public class EventInfo {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Event { get; set; }

        public object[] Arguemnts { get; set; }
    }

    [Serializable]
    public class ServiceInvokeResult : ITransferable, IResponse {
        public Guid Id { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }

        public string SrcUserId { get; set; }

        public string DestUserId { get; set; }

        public object ReturnValue { get; set; }
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

            if (info == null) return;

            context.Cancel = true;

            Console.WriteLine("收到调用{0}.{1} {2}->{3}",
                info.Name, info.Action, info.SrcUserId, info.DestUserId);

            var result = Invoke(info);

            await session.SendAsync(result);
        }

        Dictionary<string, object> services = new Dictionary<string, object>();

        public Delegate GetDelegate(Type type, Action<object, object[]> handler, object state) {
            return null;
        }

        class MyClass {

            Action<object[], object> handler;

            object state;

            public void Invoke(string a, string b) {
                object[] args = new object[] {
                    a,b
                };
                handler.Invoke(args, state);
            }
        }

        public void AddService(string name, object service) {
            services.Add(name, service);

            foreach (var e in service.GetType().GetEvents()) {
                var method = e.GetAddMethod();

                if (e.EventHandlerType == typeof(Action<string>)) {

                    method.Invoke(service, new object[] {
                        new Action<string>((s)=>{
                            Raise(session,name,e.Name,s);
                        })
                    });
                } else {

                    var handler = GetDelegate(e.EventHandlerType, (state, args) => {
                        object[] p = state as object[];
                        Raise(session, p[0] as string, p[2] as string, args);
                    }, new object[] { name, e.Name });

                    method.Invoke(service, new object[] { handler });

                    //method.Invoke(service, new object[] { });
                }
            }

        }

        public async void Raise(SocketSession session, string name, string e, params object[] args) {

            await session.SendAsync(new EventInfo {
                Event = e,
                Name = name,
                Id = Guid.NewGuid(),
                Arguemnts = args
            });
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
            result.DestUserId = info.SrcUserId;
            result.SrcUserId = info.DestUserId;

            if (services.TryGetValue(info.Name, out service)) {
                try {
                    MethodInfo method = service.GetType().GetMethod(info.Action);

                    if (method != null) {
                        try {
                            object ret = method.Invoke(service, info.Arguments);

                            Task task = ret as Task;

                            if (task != null) {
                                if (task.IsCompleted) {
                                    ret = task.GetType().GetProperty("Result").GetValue(task);
                                }
                            }

                            result.Success = true;
                            result.ReturnValue = ret;

                        } catch (Exception ex) {
                            ex = ex.InnerException;
                            result.Message = "调用出错";
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.StackTrace);
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
