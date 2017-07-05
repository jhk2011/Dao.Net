using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Net.Web {

    public interface IHttpHandler {
        Task<HttpResponse> Handle(HttpRequest request);
    }

    public class MimeType {

        public static Dictionary<string, string> mimeTypes
            = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                [".txt"] = "text/plain",
                [".cs"] = "text/plain"
            };

        public static string GetMimeType(string ext) {
            string mimeType;

            if (mimeTypes.TryGetValue(ext, out mimeType)) {
                return mimeType;
            }
            return null;
        }
    }

    public class HttpHandler : SocketHandler {

        public override async void Handle(HandleContext context) {
            HttpRequest request = context.Packet as HttpRequest;

            if (request == null) return;

            HttpResponse response = Handle(request);

            await context.Session.SendAsync(response);

            await context.Session.Close();
        }

        private HttpResponse Handle(HttpRequest request) {
            HttpResponse response = new HttpResponse();

            string contentType = "text/html; charset=utf-8";

            response.Line = "HTTP/1.1 200 OK";

            response.Headers.Add("Date", "Fri, 22 May 2009 06:07:21 GMT");
            response.Headers.Add("Connection", "close");


            string url = request.Line.Split(new char[] { ' ' })[1];

            string root = @"F:\";

            string path = Path.Combine(root, url.TrimStart('/'));

            string s = null;

            if (Directory.Exists(path)) {
                s = "<h3>文件</h3>";
                s += "<ul>";
                foreach (var file in Directory.GetFileSystemEntries(path)) {

                    var s2 = file.Replace(root, "");

                    s2 = s2.TrimStart('\\');

                    s += string.Format("<li><a href='/{0}'>{0}</a></li>", s2);
                }
                s += "</ul>";
            } else if (File.Exists(path)) {

                response.Body = File.ReadAllBytes(path);
                string ext = Path.GetExtension(path);
                contentType = MimeType.GetMimeType(ext) ?? "application/octet-stream";
            } else {
                s = "文件或路径不存在";
            }

            if (s != null) {
                response.Body = Encoding.UTF8.GetBytes(s);
            }

            if (response.Body != null) {
                response.Headers.Add("Content-Length:", response.Body.Length + "");
            }

            response.Headers.Add("Content-Type", contentType);

            return response;
        }
    }
}