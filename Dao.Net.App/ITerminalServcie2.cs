using System;
using System.Collections.Generic;

namespace Dao.Net {
    public interface ITerminalServcie2 {
        int Init();
        void Execute(int id, string command);
        void Reset(int id);
        void Close(int id);

        event Action<int, string> Error;

        event Action<int, string> Received;

        event Action<int> Closed;
    }

    public class TerminalService2 : ITerminalServcie2 {

        public event Action<int, string> Error;
        public event Action<int, string> Received;
        public event Action<int> Closed;

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
                Error?.Invoke(id, s);
            };

            terminal.Received += (s) => {
                Received?.Invoke(id, s);
            };

            terminal.Closed += () => {
                Closed?.Invoke(id);
            };

            terminal.Init();

            return id;
        }

        public void Reset(int id) {
            list[id].Reset();
        }
    }

}
