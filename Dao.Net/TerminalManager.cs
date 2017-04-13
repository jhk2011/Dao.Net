using System;
using System.Collections.Generic;
using System.Linq;

namespace Dao.Net {

    [Serializable]
    public class Base {
        public string DestUserId { get; set; }
        public string SrcUserId { get; set; }
    }

    [Serializable]
    public class TerminalInfo {
        public string Command { get; set; }
    }

    public class TransferManager : ISocketHandler {

        public SocketServer Server { get; set; }

        public void Handle(Packet packet, SocketSession session) {
            if (!string.IsNullOrEmpty(packet.DestUserId)) {
                SocketSession session2 = FindSocketSession(packet.DestUserId);
                if (session2 != null) {
                    Console.WriteLine("转发数据:{0}->{1} 类型{2}",
                        packet.ScrUserId,
                        packet.DestUserId,
                        packet.Type);

                    session2.SendAsync(packet);
                }
                throw new BreakException();
            }
        }

        private SocketSession FindSocketSession(string destUserId) {
            return Server.Sessions
                .Select(x => x.Handlers.GetHandler<UserManager>())
                .Where(x => x.UserId == destUserId)
                .Select(x => x.Session)
                .FirstOrDefault();
        }
    }

    public class TerminalManager : ISocketHandler {

        public SocketSession Session { get; set; }

        List<TerminalManager2> managers = new List<TerminalManager2>();

        public void ExecuteAsync(string command, string userid = null) {

            Packet p = new Packet(TerminalPackets.Execute);

            p.ScrUserId = Session.Handlers.GetHandler<UserManager>().UserId;
            p.DestUserId = userid;

            TerminalInfo info = new TerminalInfo {
                Command = command
            };

            p.SetObject(info);
            Session.SendAsync(p);
        }
        public void CancelAsync() {
            Packet p = new Packet(TerminalPackets.Cancel);
            Session.SendAsync(p);
        }

        public void InitAsync() {
            Packet p = new Packet(TerminalPackets.Init);
            Session.SendAsync(p);
        }

        public void CloseAsync() {
            Packet p = new Packet(TerminalPackets.Close);
            Session.SendAsync(p);
        }

        public event Action<String> Received;

        public event Action<String> Error;


        public void Cancel() {

        }

        public void Handle(Packet packet, SocketSession session) {
            if (packet.Type == TerminalPackets.Execute) {
                Execute(packet);
            }
            if (packet.Type == TerminalPackets.Init) {
                Init(packet);
            }
            if (packet.Type == TerminalPackets.Cancel) {
                Cancel();
            }
            if (packet.Type == TerminalPackets.Close) {
                Close(packet);
            }
            if (packet.Type == TerminalPackets.Receive) {
                Received?.Invoke(packet.GetString());
            }
            if (packet.Type == TerminalPackets.Error) {
                Error?.Invoke(packet.GetString());
            }
        }

        private void Close(Packet packet) {
            NewMethod(packet).Close(packet);
        }

        private void Init(Packet packet) {
            NewMethod(packet).Init(packet);
        }

        private void Execute(Packet packet) {
            TerminalManager2 manager = NewMethod(packet);
            manager.Execute(packet);
        }

        private TerminalManager2 NewMethod(Packet packet) {
            var manager = managers
                .Where(x => x.DestUserId == packet.DestUserId)
                .FirstOrDefault();

            if (manager == null) {
                manager = new TerminalManager2 {
                    SrcUserId = packet.ScrUserId,
                    DestUserId = packet.DestUserId,
                    Session = this.Session
                };
                managers.Add(manager);
            }

            return manager;
        }
    }

    public class TerminalManager2 {
        public string SrcUserId { get; set; }

        public string DestUserId { get; set; }

        public SocketSession Session { get; set; }

        Terminal terminal = new Terminal();

        public void Init(Packet packet) {
            terminal.Received += Terminal_Received;
            terminal.Error += Terminal_Error;
            terminal.Init();
        }

        public void Execute(Packet packet) {
            var info = packet.GetObject<TerminalInfo>();
            //if (terminal == null) {
            //    terminal = new Terminal();
            //    terminal.Received += Terminal_Received;
            //    terminal.Error += Terminal_Error;
            //}
            terminal.Execute(info.Command);
        }

        private void Terminal_Error(string obj) {
            Packet p = new Packet(TerminalPackets.Error);
            p.SetString(obj);
            p.ScrUserId = DestUserId;
            p.DestUserId = SrcUserId;
            Session.SendAsync(p);
        }

        private void Terminal_Received(string obj) {
            Packet p = new Packet(TerminalPackets.Receive);
            p.SetString(obj);
            p.ScrUserId = DestUserId;
            p.DestUserId = SrcUserId;
            Session.SendAsync(p);
        }

        internal void Close(Packet packet) {
            terminal.Close();
        }
    }
}
