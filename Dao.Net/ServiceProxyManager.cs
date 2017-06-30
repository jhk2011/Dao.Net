using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dao.Net {
    public class ServiceProxyManager : ISocketHandler {

        public void Handle(HandleContext context) {

            var packet = context.Packet;
            var session = context.Session;

            if (packet.Type == ServicePackets.Reply) {

                Console.WriteLine("ServiceManager：收到调用回复");

                ServiceInvokeResult result = packet.GetObject<ServiceInvokeResult>();

                InvokeCompleted?.Invoke(result);
            }
        }

        public SocketSession Session { get; set; }

        public T GetServiceProxy<T>(string service)
            where T : class {
            ServiceProxy proxy = new ServiceProxy(typeof(T), this, service);
            return proxy.GetTransparentProxy() as T;
        }

        public void InvokeAsync(
            Guid id,
            string name,
            string action,
            params object[] args) {

            Console.WriteLine("ServiceManager:发起调用{0} {1}", name, action);

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

        public Task<ServiceInvokeResult> InvokeTaskAsync(Guid id,
            string name,
            string action,
            params object[] args) {

           var tcs = new TaskCompletionSource<ServiceInvokeResult>();

            Action<ServiceInvokeResult> handler = null;

            handler = (info) => {
                if (info.Id == id) {
                    this.InvokeCompleted -= handler;
                    tcs.TrySetResult(info);
                }
            };

            //Timer t = null;

            //t = new Timer(_ => {
            //    this.InvokeCompleted -= handler;
            //    tcs.TrySetCanceled();
            //    t.Dispose();
            //});

            //t.Change(5000, Timeout.Infinite);

            this.InvokeCompleted += handler;

            this.InvokeAsync(id, name, action, args);

            return tcs.Task;
        }

        public object Invoke(ServiceInvoke info) {

            var task = InvokeTaskAsync(info.Id, info.Name, info.Action, info.Arguments);

            var result = task.Result;

            if (result.Success) {
                return result.ReturnValue;
            }

            throw new InvalidOperationException(result.Message);
        }
    }

}
