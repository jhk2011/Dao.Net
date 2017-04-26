﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Collections.ObjectModel;
using System.Threading;

namespace Dao.Net {

    public class SocketClient : SocketSession {

        public SocketClient(PacketConverter converter) :
            base(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp), converter) {

        }
        public SocketClient() : this(PacketConverter.Default) {

        }
        public Task ConnectAsync(IPAddress address, int port) {
            return Socket.ConnectTaskAsync(address, port);
        }

        public Task ConnectAsync(string host, int port) {
            return Socket.ConnectTaskAsync(host, port);
        }

        public Task DisconnectAsync() {
            return Socket.DisconnectTaskAsync(true);
        }

        protected override void OnReceived(Packet packet) {
            base.OnReceived(packet);
        }
    }


    public class TaskSocketClient : SocketClient {

        protected virtual void Raise(Action action) {
            if (action == null) {
                throw new ArgumentNullException("action");
            }
            Task.Factory.StartNew(action);
        }

        protected override void OnReceived(Packet packet) {
            Raise(() => {
                base.OnReceived(packet);
            });
        }

        protected override void OnClosed() {
            Raise(() => {
                base.OnClosed();
            });
        }
    }
    public class SyncSocketClient : TaskSocketClient {

        SynchronizationContext _synchronizationContext = SynchronizationContext.Current;

        protected override void Raise(Action action) {
            if (action == null) {
                throw new ArgumentNullException("action");
            }
            if (_synchronizationContext != null) {
                _synchronizationContext.Post(_ => {
                    action();
                }, null);
            } else {
                base.Raise(action);
            }
        }
    }
}
