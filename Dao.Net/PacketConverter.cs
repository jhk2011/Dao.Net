using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace Dao.Net {

    public class PacketConverter : IPacketConverter {

        static PacketConverter _default = new PacketConverter();

        public static PacketConverter Default { get { return _default; } }

        public async Task<Packet> ReceiveAsync(SocketSession session) {

            Packet packet = new Packet();

            byte[] header = await session.Socket.ReceiveAllAsync(8);

            int length = BitConverter.ToInt32(header, 0);
            int type = BitConverter.ToInt32(header, 4);

            byte[] buffer = null;

            packet.Type = type;

            if (length != 0) {

                if (length < 0) {
                    throw new InvalidOperationException("接收的到的长度错误");
                }
                if (length > 10 * 1024 * 1024) {
                    throw new InvalidOperationException("接收到的长度超过限制");
                }

                packet.ScrUserId = await ReadString(session);
                packet.DestUserId= await ReadString(session);
    
                buffer = await session.Socket.ReceiveAllAsync(length);

                packet.Buffer = buffer;
            }

            Console.WriteLine("Receive:{0}", type);

            return packet;
        }

        private static async Task<string> ReadString(SocketSession session) {
            byte[] buffer2 = await session.Socket.ReceiveAllAsync(4);

            int length2 = BitConverter.ToInt32(buffer2, 0);

            byte[] buffer3 = await session.Socket.ReceiveAllAsync(length2);

            return Encoding.UTF8.GetString(buffer3);
        }

        public async Task SendAsync(SocketSession session, Packet packet) {
            if (packet == null) throw new ArgumentNullException("buffer");

            Console.WriteLine("Send:{0}", packet.Type);

            using (MemoryStream ms = new MemoryStream()) {
                var buffer = packet.Buffer;
                var header1 = BitConverter.GetBytes(buffer.Length);
                var header2 = BitConverter.GetBytes(packet.Type);
                ms.Write(header1, 0, header1.Length);
                ms.Write(header2, 0, header2.Length);

                WriteString(ms, packet.ScrUserId);
                WriteString(ms, packet.DestUserId);

                ms.Write(buffer, 0, buffer.Length);
                await session.Socket.SendAllAsync(ms.ToArray());
            }
        }

        private static void WriteString(MemoryStream ms,string s) {
            var buffer3 = s == null ? Packet.EmptyBuffer :
                Encoding.UTF8.GetBytes(s);

            ms.Write(BitConverter.GetBytes(buffer3.Length), 0, 4);
            ms.Write(buffer3, 0, buffer3.Length);
        }
    }

}
