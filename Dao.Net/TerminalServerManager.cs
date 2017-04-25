using System;
using System.Collections.Generic;
using System.Linq;

namespace Dao.Net {

    [Serializable]
    public class TerminalInfo {
        public string Command { get; set; }
    }

    public class TerminalServerManager : ISocketHandler {

        private List<TerminalManager2> managers = new List<TerminalManager2>();

        public SocketSession Session { get; set; }

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
        }

        private void Close(Packet packet) {
            var t = Find(packet);
            
            t.Close(packet);

            managers.Remove(t);
        }

        private void Init(Packet packet) {
            Find(packet).Init(packet);
        }

        private void Execute(Packet packet) {
            Find(packet).Execute(packet);
        }

        private TerminalManager2 Find(Packet packet) {
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
}
