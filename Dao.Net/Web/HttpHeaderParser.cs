using System;

namespace Dao.Net.Web {
    public class HttpHeaderParser {
        public HttpHeader Parse(string s) {
            var items = s.Split(new string[] { ": " }, StringSplitOptions.None);
            return new HttpHeader(items[0], items[1]);
        }
        public string To(HttpHeader header) {
            return string.Format("{0}: {1}", header.Name, header.Value);
        }
    }
}