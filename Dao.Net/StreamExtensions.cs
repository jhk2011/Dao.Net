using System.IO;
using System.Threading.Tasks;

namespace Dao.Net {
    public static class StreamExtensions {
        public static async Task<byte[]> ReadAllAsync(this Stream stream, int count) {

            byte[] buffer = new byte[count];

            int offset = 0;

            while (offset < count) {
                offset += await stream.ReadAsync(buffer, offset, count - offset);
            }

            return buffer;
        }

        public static async Task WriteAllAsync(this Stream stream, byte[] buffer) {
            await stream.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}