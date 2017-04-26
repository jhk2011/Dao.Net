﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Net {
    public interface ITerminalService {
        void Execute(string command);
        void Cancel();
        void Init();
        void Close();
    }

    public interface ITerminalCallbackService {
        void OnReceive(string s);
        void OnError(string s);
    }

    public class TerminalCallbackService : ITerminalCallbackService {

        public event Action<string> Error;

        public event Action<string> Received;

        public void OnError(string s) {
            Error?.Invoke(s);
        }

        public void OnReceive(string s) {
            Received?.Invoke(s);
        }
    }

    public class TerminalData {
        public string DestUserId { get; set; }
        public Terminal Terminal { get; set; }
        public ITerminalCallbackService Callback { get; set; }
    }

    public class TerminalService : ITerminalService {

        List<TerminalData> dataList = new List<TerminalData>();

        Terminal t = new Terminal();
        public void Cancel() {
            Find().Terminal.Cancel();
        }

        public void Close() {
            var d = Find();

            d.Terminal.Close();

            dataList.Remove(d);
        }

        public void Execute(string command) {
            Find().Terminal.Execute(command);
        }

        public void Init() {
            var d = Find();
            d.Terminal.Error += T_Error;
            d.Terminal.Received += T_Received;
            d.Terminal.Init();
        }

        private void T_Received(Terminal t, string obj) {
            var d = Find(t);
            d.Callback.OnReceive(obj);
        }

        private void T_Error(Terminal t, string obj) {
            var d = Find(t);
            d.Callback.OnError(obj);
        }

        public TerminalData Find() {

            string destUserId = SocketContext.Current.Packet.DestUserId;
            string srcUserId = SocketContext.Current.Packet.SrcUserId;
            var serviceManager = SocketContext.Current.Session.Handlers
                .GetHandler<ServiceManager>();

            TerminalData data = dataList.Where(x => x.DestUserId == destUserId)
                .FirstOrDefault();
            if (data == null) {
                data = new TerminalData {
                    DestUserId = destUserId,
                    Terminal = new Terminal(),
                    Callback = serviceManager
                        .GetServiceProxy<ITerminalCallbackService>("terminalCallback", srcUserId)
                };
                dataList.Add(data);
            }
            return data;
        }
        public TerminalData Find(Terminal t) {
            TerminalData data = dataList.Where(x => x.Terminal == t)
                .FirstOrDefault();
            return data;
        }
    }

}
