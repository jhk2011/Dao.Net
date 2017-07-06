using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Net {

    public interface ITerminalService {
        int Init();
        void Execute(int id, string command);
        void Reset(int id);
        void Close(int id);
    }

    public interface ITermainalCallbackService {
        void OnError(int id, string s);
        void OnRecieve(int id, string s);
    }

    public class TerminalCallbackService : ITermainalCallbackService {

        public event Action<int, string> Error;

        public event Action<int, string> Receive;

        public void OnError(int id, string s) {
            Error?.Invoke(id, s);
        }

        public void OnRecieve(int id, string s) {
            Receive?.Invoke(id, s);
        }
    }

    public class TerminalService : ITerminalService {

        ITermainalCallbackService callback;

        public TerminalService(ITermainalCallbackService callback) {
            this.callback = callback;
        }


        List<Terminal> list = new List<Terminal>();

        public void Close(int id) {
            list[id].Close();
        }

        public void Execute(int id, string command) {

            list[id].Execute(command);
        }

        public int Init() {

            Terminal terminal = new Terminal();

            int id = list.Count;

            list.Add(terminal);

            terminal.Error += (s) => {
                callback.OnError(id, s);
            };

            terminal.Received += (s) => {
                callback.OnRecieve(id, s);
            };

            terminal.Init();

            return id;
        }

        public void Reset(int id) {
            list[id].Reset();
        }

    }
}
