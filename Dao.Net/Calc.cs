using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Net {

    public interface ICalc {
        double Add(int a, int b);
    }
    public interface ICalcCallback {
        void OnInfo(string s);
    }

    public class Calc : ICalc {

        public double Add(int a, int b) {
            Info("计算前");
            double result = a + b;
            Info("计算后");
            return result;
        }

        public void Info(string s) {
            var ctx = SocketContext.Current;

            var serviceManager = ctx.Session.Handlers.GetHandler<ServiceManager>();

            var callback = serviceManager.GetServiceProxy<ICalcCallback>("calcCallback",
                ctx.Packet.SrcUserId);

            callback?.OnInfo(s);

        }
    }

    public class CalcCallback : ICalcCallback {

        public event Action<string> Info;
        public void OnInfo(string s) {
            Info?.Invoke(s);
        }
    }
}
