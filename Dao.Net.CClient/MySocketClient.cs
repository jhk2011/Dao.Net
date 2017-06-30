namespace Dao.Net.CClient {
    public class MySocketClient : SocketClient {

        public ICalc calc;

        public ITerminalService Terminal { get; set; }

        public TerminalCallbackService TerminalCallback { get; set; }

        public MySocketClient() {

            this.Handlers = new SocketHandlerCollection();

            ServiceManager serviceManager = new ServiceManager();

            serviceManager.AddService("calct", new CalcTip());

            this.Handlers.Add(serviceManager);

            var serviceProxyManager = new ServiceProxyManager() {
                Session = this
            };

            TerminalCallback = new TerminalCallbackService();

            serviceManager.AddService("tc", TerminalCallback);

            calc = serviceProxyManager.GetServiceProxy<ICalc>("calc");

           
            Terminal = serviceProxyManager.GetServiceProxy<ITerminalService>("t");

            this.Handlers.Add(serviceProxyManager);
        }

        protected override void OnReceived(Packet packet) {
            base.OnReceived(packet);
            System.Console.WriteLine("--收到:{0}",packet.Type);
        }
    }
}
