using System;

namespace Dao.Net {
    public interface ICalc2 {
        double Add(double x, double y);

        event Action<string> Added;
    }

    public class Calc2 : ICalc2 {

        public event Action<string> Added;

        public double Add(double x, double y) {

            Added?.Invoke(string.Format("{0}+{1}", x, y));

            return x + y;
        }
    }

}
