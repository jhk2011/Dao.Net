using System.Net.Sockets;

namespace Dao.Net.Server {
    class MySocketServer : SocketServer {

        public MySocketServer() {

        }

        public MySocketServer(SocketListener l) : base(l) {

        }

        protected override SocketSession GetSocketSession(Socket client) {
            return new MyServerSession(client, this);
        }

        protected override void OnAccepted(SocketSession session) {

            //ServiceManager serviceManager = session.Handlers.GetHandler<ServiceManager>();

            //if (serviceManager != null) {
            //    foreach (var service in serviceManager.GetServices()) {
            //        ISessionCallback callback = service as ISessionCallback;
            //        if (callback != null) {
            //            callback.OnAccept(session);
            //        }
            //    }
            //}

            base.OnAccepted(session);
        }

        protected override void OnClosed(SocketSession session) {

            //ServiceManager serviceManager = session.Handlers.GetHandler<ServiceManager>();

            //foreach (var service in serviceManager.GetServices()) {
            //    ISessionCallback callback = service as ISessionCallback;
            //    if (callback != null) {
            //        callback.OnClose(session);
            //    }
            //}

            base.OnClosed(session);
        }

        protected override void OnReceived(SocketSession session, Packet packet) {
            base.OnReceived(session, packet);
        }
    }
}
