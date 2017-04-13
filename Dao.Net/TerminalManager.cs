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
    public class TerminalInfo : Base {
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

        public event Action<String> Received;

        public event Action<String> Error;

        Terminal terminal = null;
        public void Execute(string command) {

            if (terminal == null) {
                terminal = new Terminal();
                terminal.Received += Terminal_Received;
                terminal.Error += Terminal_Error;
            }
            terminal.Execute(command);
        }

        public void Cancel() {
            terminal.Cancel();
        }
        private void Terminal_Error(string obj) {
            Packet p = new Packet(TerminalPackets.Error);
            p.SetString(obj);
            Session.SendAsync(p);
        }

        private void Terminal_Received(string obj) {
            Packet p = new Packet(TerminalPackets.Receive);
            p.SetString(obj);
            Session.SendAsync(p);
        }

        public void Handle(Packet packet, SocketSession session) {
            if (packet.Type == TerminalPackets.Execute) {
                TerminalInfo info = packet.GetObject<TerminalInfo>();
                //Execute(info.Command);
                Execute(info,packet.ScrUserId,packet.DestUserId);
            }
            if (packet.Type == TerminalPackets.Cancel) {
                Cancel();
            }
            if (packet.Type == TerminalPackets.Receive) {
                Received?.Invoke(packet.GetString());
            }
            if (packet.Type == TerminalPackets.Error) {
                Error?.Invoke(packet.GetString());
            }
        }

        private void Execute(TerminalInfo info,string src,string dest) {
            var manager = managers
                .Where(x => x.DestUserId == info.DestUserId)
                .FirstOrDefault();

            if (manager == null) {
                manager = new TerminalManager2 {
                    SrcUserId = dest,
                    DestUserId = src,
                    Session = this.Session
                };
                managers.Add(manager);
            }
            manager.Execute(info.Command);
        }
    }

    public class TerminalManager2 {
        public string SrcUserId { get; set; }

        public string DestUserId { get; set; }

        public SocketSession Session { get; set; }

        Terminal terminal = null;
        public void Execute(string command) {
            if (terminal == null) {
                terminal = new Terminal();
                terminal.Received += Terminal_Received;
                terminal.Error += Terminal_Error;
            }
            terminal.Execute(command);
        }

        private void Terminal_Error(string obj) {
            Packet p = new Packet(TerminalPackets.Error);
            p.SetString(obj);
            p.ScrUserId = SrcUserId;
            p.DestUserId = DestUserId;
            Session.SendAsync(p);
        }

        private void Terminal_Received(string obj) {
            Packet p = new Packet(TerminalPackets.Receive);
            p.SetString(obj);
            p.ScrUserId = SrcUserId;
            p.DestUserId = DestUserId;
            Session.SendAsync(p);
        }
    }
}
