using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Net
{

        public class ByteArray
        {
            public ByteArray(byte[] buffer, int offset, int size)
            {
                Buffer = buffer;
                Offset = offset;
                Size = size;
            }

            public ByteArray(byte[] buffer)
                : this(buffer, 0, buffer.Length)
            {

            }

            public byte[] Buffer { get; private set; }
            public int Offset { get; private set; }
            public int Size { get; private set; }
        }

        public static class SocketExtensions
        {
            public static IAsyncResult BeginSend(this Socket socket, ByteArray arg, AsyncCallback callback, object state)
            {
                return socket.BeginSend(arg.Buffer, arg.Offset, arg.Size, SocketFlags.None, callback, state);
            }
            public static IAsyncResult BeginReceive(this Socket socket, ByteArray arg, AsyncCallback callback, object state)
            {
                return socket.BeginReceive(arg.Buffer, arg.Offset, arg.Size, SocketFlags.None, callback, state);
            }

            public static async Task<int> SendTaskAsync(this Socket socket, byte[] buffer, int offset, int size)
            {
               return await Task.Factory.FromAsync<ByteArray, int>(socket.BeginSend, socket.EndSend,
                    new ByteArray(buffer, offset, size), null).ConfigureAwait(false);
            }
            public static Task<int> SendTaskAsync(this Socket socket, byte[] buffer)
            {
                return socket.SendTaskAsync(buffer, 0, buffer.Length);
            }
            public async static Task<int> ReceiveTaskAsync(this Socket socket, byte[] buffer, int offset, int size)
            {
               return await Task.Factory.FromAsync<ByteArray, int>(socket.BeginReceive, socket.EndReceive,
                    new ByteArray(buffer, offset, size), null).ConfigureAwait(false);
            }

            public static Task<int> ReceiveTaskAsync(this Socket socket, byte[] buffer)
            {
                return socket.ReceiveTaskAsync(buffer, 0, buffer.Length);
            }

            public static async Task SendAllAsync(this Socket socket, byte[] buffer)
            {
                int count = 0;
                int offset = 0;
                int size = buffer.Length;

                while (count < size)
                {
                    int n = await socket.SendTaskAsync(buffer, offset, size).ConfigureAwait(false);
                    count += n;
                    size -= n;
                }
            }

            public static async Task<byte[]> ReceiveAllAsync(this Socket socket, int length)
            {
                byte[] buffer = new byte[length];

                int count = 0;
                int offset = 0;
                int size = buffer.Length;

                while (count < size)
                {
                    int n = await socket.ReceiveTaskAsync(buffer, offset, size).ConfigureAwait(false);
                    count += n;
                    size -= n;
                }
                return buffer;
            }

            public static Task ConnectTaskAsync(this Socket socket, string host, int port)
            {
                return Task.Factory.FromAsync(socket.BeginConnect, socket.EndConnect,
                    host, port, null);
            }

            public static Task ConnectTaskAsync(this Socket socket, IPAddress address, int port)
            {
                return Task.Factory.FromAsync(socket.BeginConnect, socket.EndConnect,
                    address, port, null);
            }

            public static Task<Socket> AcceptTaskAsync(this Socket socket)
            {
                return Task.Factory.FromAsync<Socket>(socket.BeginAccept, socket.EndAccept,
                    null);
            }
            public static Task DisconnectTaskAsync(this Socket socket, bool reuseSocket)
            {
                return Task.Factory.FromAsync(socket.BeginDisconnect, socket.EndDisconnect,
                    reuseSocket, null);
            }

            public static void SetKeepAlive(this Socket socket, bool on, int time, int interval)
            {
                if (socket == null) throw new ArgumentNullException("socket");
                byte[] buffer = new byte[12];
                BitConverter.GetBytes(on ? 1 : 0).CopyTo(buffer, 0);
                BitConverter.GetBytes(time).CopyTo(buffer, 4);
                BitConverter.GetBytes(interval).CopyTo(buffer, 8);
                socket.IOControl(IOControlCode.KeepAliveValues, buffer, null);
            }
        }

}
