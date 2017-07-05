using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Net {
    public class Terminal {

        Process process;

        public void Init() {

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;

            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;

            process = new Process();

            process.StartInfo = startInfo;
            process.Start();

            //process.EnableRaisingEvents = true;

            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.OutputDataReceived += Process_OutputDataReceived;
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        public void Execute(string commad) {
            process.StandardInput.Write(commad);
        }

        public void Reset() {
            Close();
            Init();
        }


        public event Action<Terminal, String> Received;

        public event Action<Terminal, String> Error;

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e) {
            Received?.Invoke(this, e.Data);
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e) {
            Error?.Invoke(this, e.Data);
        }

        public void Close() {

            if (process == null) return;

            process.CancelOutputRead();
            process.CancelErrorRead();
            process.Kill();
            process.Dispose();

            process = null;
        }
    }

    public interface ITerminalService {
        int Init();
        void Execute(int id, string command);
        void Reset(int id);
        void Close(int id);
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

            Terminal t = new Terminal();

            int id = list.Count;

            list.Add(t);

            t.Error += (tt, s) => {
                callback.OnError(id, s);
            };

            t.Received += (tt, s) => {
                Console.WriteLine("Received:{0}", s);
                callback.OnRecieve(id, s);
                Console.WriteLine("Received callback:{0}", s);
            };

            t.Init();

            return id;
        }

        public void Reset(int id) {
            list[id].Reset();
        }

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
    public interface ITermainalCallbackService {
        void OnError(int id, string s);
        void OnRecieve(int id, string s);
    }

}
