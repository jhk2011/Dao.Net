namespace Dao.Net {
    public class TerminalManager2 {
        public string SrcUserId { get; set; }

        public string DestUserId { get; set; }

        public SocketSession Session { get; set; }

        Terminal terminal = new Terminal();

        bool init;

        public void Init(Packet packet) {
            if (init) return;
            terminal.Received += Terminal_Received;
            terminal.Error += Terminal_Error;
            terminal.Init();
            init = true;
        }

        public void Execute(Packet packet) {
            var info = packet.GetObject<TerminalInfo>();
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
            terminal.Received -= Terminal_Received;
            terminal.Error -= Terminal_Error;
            terminal.Close();
            init = false;
        }
    }
}
