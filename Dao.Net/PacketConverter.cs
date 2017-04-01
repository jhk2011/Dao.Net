using System;
using System.IO;
using System.Threading.Tasks;


namespace Dao.Net {

    public class PacketConverter : IPacketConverter {

        static PacketConverter _default = new PacketConverter();

        public static PacketConverter Default { get { return _default; } }

        public async Task<Packet> ReceiveAsync(SocketSession session) {
            byte[] header = await session.Socket.ReceiveAllAsync(8);

            int length = BitConverter.ToInt32(header, 0);
            int type = BitConverter.ToInt32(header, 4);

            byte[] buffer = null;

            if (length != 0) {

                if (length < 0) {
                    throw new InvalidOperationException("接收的到的长度错误");
                }
                if (length > 10 * 1024 * 1024) {
                    throw new InvalidOperationException("接收到的长度超过限制");
                }

                buffer = await session.Socket.ReceiveAllAsync(length);
            }

            Console.WriteLine("Receive:{0}", type);

            return new Packet(type, buffer);
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
                ms.Write(buffer, 0, buffer.Length);
                await session.Socket.SendAllAsync(ms.ToArray());
            }
        }
    }

}
