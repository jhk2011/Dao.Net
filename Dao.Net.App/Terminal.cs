using System;
using System.Diagnostics;

namespace Dao.Net {
    public class Terminal:IDisposable {

        Process process;

        public void Init() {

            if (process != null) return;

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


            process.Exited += Process_Exited;
            process.EnableRaisingEvents = true;
            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.OutputDataReceived += Process_OutputDataReceived;
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        private void Process_Exited(object sender, EventArgs e) {
            Closed?.Invoke();
        }

        public void Execute(string commad) {
            process.StandardInput.Write(commad);
        }

        public void Reset() {
            Close();
            Init();
        }


        public event Action<String> Received;

        public event Action<String> Error;

        public event Action Closed;
        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e) {
            Received?.Invoke(e.Data);
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e) {
            Error?.Invoke(e.Data);
        }

        public void Close() {

            if (process == null) return;

            process.CancelOutputRead();
            process.CancelErrorRead();
            process.Kill();
            process.Dispose();

            process = null;
        }

        public void Dispose() {
            Console.WriteLine("--Dispose");
            Close();
        }
    }

}
