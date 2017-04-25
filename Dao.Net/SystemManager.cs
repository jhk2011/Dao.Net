using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Net {
    public class SystemManager : ISocketHandler {

        public string Id { get; set; }

        public SocketSession Session { get; set; }

        public SocketSessionCollection Sessions { get; set; }

        public void Handle(Packet packet, SocketSession session) {
            if (packet.Type == SysPackets.Join) {
                Id = packet.GetObject<string>();
                Join?.Invoke(Id);
            }
            if (packet.Type == SysPackets.Leave) {
                Id = packet.GetObject<string>();
                Leave?.Invoke(Id);
            }
        }

        public event Action<string> Join;
        public event Action<string> Leave;

        public event Action<string[]> List;

        public void JoinAsync() {
            Id = Guid.NewGuid() + "";
            Packet p = new Packet(SysPackets.Join);
            p.SetObject(Id);
            Sessions.Broadcast(p);
            Console.WriteLine("Join");
        }

        public void LeaveAsync() {
            Packet p = new Packet(SysPackets.Leave);
            p.SetObject(Id);
            Sessions.Broadcast(p);
            Console.WriteLine("Leave");
        }

        public void ListAsync() {
            Packet p = new Packet(SysPackets.List);
            p.SetObject(Id);
            Sessions.Broadcast(p);
            Console.WriteLine("Leave");
        }
    }
}
