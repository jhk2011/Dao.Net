using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Net {
    [Serializable]
    public class PacketPart {
        public Guid Id { get; set; }
        public int Index { get; set; }
        public byte[] Buffer { get; set; }
    }

    [Serializable]
    public class PacketEnd {
        public Guid Id { get; set; }
    }


    public class PacketPartPackets {
        public const int PacketPart = -2;
        public const int PacketEnd = -1;
    }

    public class PacketConvert2 : IPacketConverter {

        IPacketConverter innerConverter;

        List<Packet> packets = new List<Packet>();

        public async Task<Packet> ReceiveAsync(SocketSession session) {
            Packet packet = await innerConverter.ReceiveAsync(session);
            while (packet.Type < 0) {
                packets.Add(packet);
                packet = await innerConverter.ReceiveAsync(session);
                if (packet.Type == PacketPartPackets.PacketEnd) {
                    Packet packet2 = GetPacket();
                    return packet2;
                }
            }

            return packet;
        }

        private Packet GetPacket() {
            throw new NotImplementedException();
        }

        public async Task SendAsync(SocketSession session, Packet packet) {
            var packets = Split(packet);
            if (packets == null) {
                await innerConverter.SendAsync(session, packet);
            } else {
                foreach (var packet0 in packets) {
                    await innerConverter.SendAsync(session, packet0);
                }
            }
        }

        private List<Packet> Split(Packet packet) {
            if (packet.Buffer.Length > 2048) {
                return null;
            }
            return null;
        }
    }
}
