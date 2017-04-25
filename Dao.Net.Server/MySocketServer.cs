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
            base.OnAccepted(session);

            

            //SystemManager systemManager = new SystemManager() {
            //    Session = session,
            //    Sessions = Sessions
            //};
            //systemManager.JoinAsync();
            //session.Handlers.Add(systemManager);
            System.Console.WriteLine(session.Socket.RemoteEndPoint);
        }

        protected override void OnClosed(SocketSession session) {
            //SystemManager systemmanager = session.Handlers.GetHandler<SystemManager>();
            //systemmanager?.LeaveAsync();
            base.OnClosed(session);
        }

        protected override void OnReceived(SocketSession session, Packet packet) {
            base.OnReceived(session, packet);
        }
    }
}
