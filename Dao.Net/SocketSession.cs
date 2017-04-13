using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dao.Net {

    public class SocketSession {

        Socket _socket;
        IPacketConverter _converter;

        public Socket Socket
        {
            get { return _socket; }
            private set { _socket = value; }
        }

        public SocketSession(Socket socket, IPacketConverter converter) {
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        public SocketSession(Socket socket) : this(socket, PacketConverter.Default) {

        }

        public event EventHandler<ReceivedEventArgs> Received;

        public event EventHandler Closed;

        public PacketHandlerCollection Handlers { get; set; }

        public virtual Task SendAsync(Packet packet) {
            if (packet == null) throw new ArgumentNullException("buffer");
            return _converter.SendAsync(this, packet);
        }

        public Task SendAsync(byte[] packet) {
            if (packet == null) throw new ArgumentNullException("buffer");
            return SendAsync(new Packet(0, packet));
        }

        public Task SendAsync(int type) {
            Packet packet = new Packet(type);
            return SendAsync(packet);
        }

        public Task SendAsync(string s) {
            Packet packet = new Packet();
            packet.SetString(s);
            return SendAsync(packet);
        }

        public Task<Packet> ReceiveAsync() {
            return _converter.ReceiveAsync(this);
        }

        public async void StartReceive() {

            Packet packet = null;
            try {
                packet = await ReceiveAsync();
            } catch (Exception ex) {
                Console.WriteLine("Receive Error:{0}", ex.Message);
                Console.WriteLine(ex.StackTrace);
                OnClosed();
            }
            if (packet != null) {
                OnReceived(packet);
                StartReceive();
            }
        }

        protected virtual void OnReceived(Packet packet) {
            Received?.Invoke(this, new ReceivedEventArgs(this, packet));
            Handlers?.Handle(packet, this);
        }

        protected virtual void OnClosed() {
            Closed?.Invoke(this, EventArgs.Empty);
        }


        //public Task<Packet> InvokeAsync(Packet packet, Func<Packet, bool> predicate) {
        //    TaskCompletionSource<Packet> tcs = new TaskCompletionSource<Packet>();

        //    this.Received += (s, e) => {
        //        if (predicate(e.Pakcet)) {
        //            tcs.TrySetResult(e.Pakcet);
        //        }
        //    };

        //    this.Closed += (s, e) => {
        //        tcs.TrySetException(new ObjectDisposedException("会话已关闭"));
        //    };

        //    var t = this.SendAsync(packet);

        //    return tcs.Task;
        //}

        //public void Reply(int packetType, Func<Packet, Packet> func) {
        //    this.Received += (s, e) => {
        //        if (e.Pakcet.Type == packetType) {
        //            Packet p = func(e.Pakcet);
        //            var t = this.SendAsync(p);
        //        }
        //    };
        //}

        //public Task<Packet> InvokeAsync(Packet packet, int replyType) {
        //    return InvokeAsync(packet, x => x.Type == replyType);
        //}
    }

}
