namespace Dao.Net.CClient {
    public class MySocketClient : SyncSocketClient {

        public ICalc calc;

        public ITerminalService Terminal { get; set; }

        public TerminalCallbackService TerminalCallback { get; set; }

        public ServiceClientHandler serviceClientHandler;
        public MySocketClient() {

            ServiceHandler serviceManager = new ServiceHandler();

            serviceClientHandler = new ServiceClientHandler() {
                Session = this
            };

            this.Handlers.Add(serviceManager);

            this.Handlers.Add(serviceClientHandler);

           


            TerminalCallback = new TerminalCallbackService();

            serviceManager.AddService("tc", TerminalCallback);

            Terminal = serviceClientHandler.GetServiceProxy<ITerminalService>("t");


            serviceManager.AddService("calcc", new CalcCallback());

            calc = serviceClientHandler.GetServiceProxy<ICalc>("calc");

        }

    }
}
