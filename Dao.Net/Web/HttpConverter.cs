using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Dao.Net.Web {
    public class HttpConverter : ISocketConverter {

        SocketSession session;

        NetworkStream ns;

        HttpHeaderParser headParser = new HttpHeaderParser();

        public HttpConverter(SocketSession session) {
            this.session = session;
            this.ns = new NetworkStream(session.Socket);

        }

        public async Task<object> ReadAsync() {

            var reader = new StreamReader(ns);

            HttpRequest request = new HttpRequest();

            string s = await reader.ReadLineAsync();

            request.Line = s;

            Console.WriteLine(s);

            string header = await reader.ReadLineAsync();

            while (header != "") {
                request.Headers.Add(headParser.Parse(header));
                header = await reader.ReadLineAsync();
                Console.WriteLine(header);
            }

            return request;
        }

        public async Task WriteAsync(object packet) {

            if (packet == null) throw new ArgumentNullException("packet");

            HttpResponse response = packet as HttpResponse;

            if (response == null) throw new ArgumentException();

            var writer = new StreamWriter(ns);

            await writer.WriteLineAsync(response.Line);

            Console.WriteLine(response.Line);

            foreach (var header in response.Headers) {
                string s = headParser.To(header);
                await writer.WriteLineAsync(s);
                Console.WriteLine(s);
            }

            await writer.WriteLineAsync("");

            await writer.FlushAsync();

            if (response.Body != null) {
                await ns.WriteAllAsync(response.Body);
            }

            //await writer.WriteLineAsync("");
        }
    }
}