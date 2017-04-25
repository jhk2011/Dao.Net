using System;

namespace Dao.Net {
    public class TerminalClientManager : ISocketHandler {

        public SocketSession Session { get; set; }

        public void ExecuteAsync(string command) {

            Packet p = new Packet(TerminalPackets.Execute);

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
            if (packet.Type == TerminalPackets.Receive) {
                Received?.Invoke(packet.GetString());
            }
            if (packet.Type == TerminalPackets.Error) {
                Error?.Invoke(packet.GetString());
            }
        }
    }
}
