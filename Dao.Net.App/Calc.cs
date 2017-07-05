using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dao.Net {
    public interface ICalc {
        double Add(double x, double y);

        Task<double> AddAsync(double x, double y);
    }


    public class Calc : ICalc {

        public ICalcback callback;

        public double Add(double x, double y) {

            try {
                for (int i = 0; i < 3; i++) {
                    callback.Info(string.Format("{0}+{1}", x, y));
                }
            } catch (Exception) {

            }

            return x + y;
        }

        public Task<double> AddAsync(double x, double y) {
            return Task.FromResult(x + y);
        }
    }

    public interface ICalcback {
        void Info(string s);
    }

    public class CalcCallback : ICalcback {
        public void Info(string s) {
            Console.WriteLine(s);
        }
    }
}
