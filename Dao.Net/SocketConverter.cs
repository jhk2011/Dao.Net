using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Net {


    public interface ISocketConverter {
        Task<object> ReadAsync();
        Task WriteAsync(object packet);
    }

    public class SocketConverter : ISocketConverter {

        SocketSession session;

        NetworkStream ns;

        public SocketConverter(SocketSession session) {
            this.session = session;
            this.ns = new NetworkStream(session.Socket);
        }

        public async Task<object> ReadAsync() {

            byte[] buffer = await ns.ReadAllAsync(4);

            int length = BitConverter.ToInt32(buffer, 0);

            Console.WriteLine("读取长度{0}", length);

            buffer = await ns.ReadAllAsync(length);

            Console.WriteLine("读取内容{0}", length);

            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream ms = new MemoryStream(buffer)) {
                object obj = formatter.Deserialize(ms);
                Console.WriteLine("读取对象类型:{0}", obj.GetType().Name);
                return obj;
            }
        }

        public async Task WriteAsync(object packet) {

            if (packet == null) throw new ArgumentNullException("packet");

            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream ms = new MemoryStream()) {

                formatter.Serialize(ms, packet);

                int length = (int)ms.Position;

                byte[] lengthBuffer = BitConverter.GetBytes((int)ms.Position);

                using (MemoryStream ms2 = new MemoryStream()) {

                    ms2.Write(lengthBuffer, 0, lengthBuffer.Length);

                    ms.Position = 0;

                    await ms.CopyToAsync(ms2);

                    ms2.Position = 0;

                    await ms2.CopyToAsync(ns);

                    Console.WriteLine("发送长度{0}", length);

                    Console.WriteLine("发送内容{0}", length);

                    Console.WriteLine("发送对象类型:{0}", packet.GetType().Name);
                }

            }
        }
    }

}