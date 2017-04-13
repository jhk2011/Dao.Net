using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dao.Net {

    public static class Win32Api {
        // Methods
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AttachConsole(uint dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern bool FreeConsole();
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll")]
        private static extern bool GenerateConsoleCtrlEvent(CtrlTypes dwCtrlEvent, uint dwProcessGroupId);
        public static void SendCtrlC(Process proc) {
            //FreeConsole();
            if (AttachConsole((uint)proc.Id)) {
                SetConsoleCtrlHandler(null, true);
                GenerateConsoleCtrlEvent(CtrlTypes.CTRL_C_EVENT, 0);
                Thread.Sleep(100);
                SetConsoleCtrlHandler(null, false);
                FreeConsole();
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool state);

        // Nested Types
        private enum CtrlTypes : uint {
            CTRL_BREAK_EVENT = 1,
            CTRL_C_EVENT = 0,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        [DllImport("user32.dll", EntryPoint = "keybd_event")]

        public static extern void keybd_event(

            byte bVk, //虚拟键值

            byte bScan,// 一般为0

            int dwFlags, //这里是整数类型 0 为按下，2为释放

            int dwExtraInfo //这里是整数类型 一般情况下设成为 0 
            );

        [DllImport("user32.dll")]
        public static extern int PostMessage(IntPtr hwnd, int wMsg,
            uint wParam, uint lParam);

        public static void Cancel2(IntPtr hwnd) {
            keybd_event(0x11, 0, 0, 0);
            PostMessage(hwnd, 0x0100, 0x43, 0x001E0001);
            PostMessage(hwnd, 0x0101, 0x43, 0xC01E0001);
            keybd_event(0x11, 0, 2, 0);
            //        keybd_event VK_SHIFT, &H2A, 0, 0  ' 模拟按下SHIFT键，&H2A是VK_SHIFT的扫描码
            //PostMessage hWndMsg, WM_KEYDOWN, VK_A, &H001E0001 ' 模拟按下 A 键，SHIFT+A产生一个大写A字符
            //PostMessage hWndMsg, WM_KEYUP, VK_A, &HC01E0001   ' 模拟抬起 A 键
            //keybd_event VK_SHIFT, &H2A, KEYEVENTF_KEYUP, 0    ' 模拟抬起 SHIFT 键
        }
    }


    class Terminal {

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

            process.EnableRaisingEvents = true;

            Task.Factory.StartNew(() => {
                BinaryReader reader = new BinaryReader(
                    process.StandardOutput.BaseStream, Encoding.Default);
                while (!process.StandardOutput.EndOfStream) {

                    char ch = reader.ReadChar();
                    Received?.Invoke(ch.ToString());

                    //string s = process.StandardOutput.ReadLine();

                    //Received?.Invoke(s);
                }
            });

            Task.Factory.StartNew(() => {
                while (!process.StandardOutput.EndOfStream) {
                    string s = process.StandardError.ReadLine();
                    Error?.Invoke(s);
                }
            });

            //process.ErrorDataReceived += Process_ErrorDataReceived;
            //process.OutputDataReceived += Process_OutputDataReceived;
            //process.BeginOutputReadLine();
            //process.BeginErrorReadLine();
        }

        public void Execute(string commad) {
            process.StandardInput.Write(commad);
        }

        public void Cancel() {

        }


        public event Action<String> Received;

        public event Action<String> Error;

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e) {
            Received?.Invoke(e.Data);
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e) {
            Error?.Invoke(e.Data);
        }

        internal void Close() {
            process.Kill();
            process = null;
        }
    }

    class TerminalPackets {
        public const int Execute = 5001;
        public const int Receive = 5002;
        public const int Error = 5003;
        public const int Cancel = 5004;
        public const int Init = 5005;
        public const int Close = 5006;
    }
}
