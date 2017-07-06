using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dao.Net {
    public class ServiceClientHandler : SocketHandler {

        public override void Handle(HandleContext context) {

            var packet = context.Packet;
            var session = context.Session;

            var response = packet as IResponse;

            if (response != null) {
                Console.WriteLine("调用回复");

                Invoked?.Invoke(response);
            }

            var e = packet as EventInfo;

            if (e != null) {

                Console.WriteLine("事件");

                Raised?.Invoke(e);
            }
        }

        public SocketSession Session { get; set; }

        public T GetServiceProxy<T>(string service, string userid = null, string instance = null)
            where T : class {
            ServiceProxy proxy = new ServiceProxy(typeof(T), this, service, userid, instance);
            return proxy.GetTransparentProxy() as T;
        }

        public async void InvokeAsync(ServiceInvoke info) {

            Console.WriteLine("发起调用{0}.{1}", info.Name, info.Action);

            await Session.SendAsync(info);
        }

        public event Action<IResponse> Invoked;

        public event Action<EventInfo> Raised;

        public Task<IResponse> InvokeTaskAsync(ServiceInvoke info) {

            var tcs = new TaskCompletionSource<IResponse>();

            Action<IResponse> handler = null;

            handler = (result) => {

                if (result.Id == info.Id) {
                    this.Invoked -= handler;
                    tcs.TrySetResult(result);
                }
            };

            //Timer t = null;

            //t = new Timer(_ => {
            //    this.InvokeCompleted -= handler;
            //    tcs.TrySetCanceled();
            //    t.Dispose();
            //});

            //t.Change(5000, Timeout.Infinite);

            this.Invoked += handler;

            this.InvokeAsync(info);

            return tcs.Task;
        }

        public async Task<T> InvokeTaskAsync2<T>(ServiceInvoke info) {

            var result = await InvokeTaskAsync(info);

            if (result.Success) {

                var invokeResult = result as ServiceInvokeResult;

                return (T)invokeResult.ReturnValue;
            }

            throw new InvalidOperationException(result.Message);
        }

        public object Invoke(ServiceInvoke info) {

            var task = InvokeTaskAsync(info);

            while (!task.IsCompleted) {
                SynchronizationContext ctx = SynchronizationContext.Current;

                if (ctx != null) {
                    if (ctx is WindowsFormsSynchronizationContext) {
                        Application.DoEvents();
                    }
                }
                //Console.WriteLine("IsCompleted");
                Thread.Sleep(1);
            }

            var result = task.Result;

            if (result.Success) {

                var invokeResult = result as ServiceInvokeResult;

                return invokeResult.ReturnValue;
            }

            throw new InvalidOperationException(result.Message);
        }

        public async void Subscribe(Subscribe info) {
            await Session.SendAsync(info);
        }
    }

}
