using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dao.Net {
    public interface ICalc {
        double Add(double x, double y);
    }

    public class Calc : ICalc {

        public ICalcTip tip;

        public double Add(double x, double y) {

            try {
               tip.Info(string.Format("{0}+{1}", x, y));
            } catch (Exception) {

            }
           

            return x + y;
        }
    }

    public interface ICalcTip {
        void Info(string s);
    }

    public class CalcTip : ICalcTip {
        public void Info(string s) {
            //Thread.Sleep(200);
            Console.WriteLine(s);
        }
    }
}
