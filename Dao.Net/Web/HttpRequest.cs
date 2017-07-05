using System;
using System.Collections.Generic;

namespace Dao.Net.Web {
    public class HttpRequest {
        public string Line { get; set; }
        public HttpHeaderCollection Headers { get; set; }
        public byte[] Body { get; set; }

        public HttpRequest() {
            Headers = new HttpHeaderCollection();
        }

        public string ContentType
        {
            get
            {
                return Headers["ContentType"];
            }
            set
            {
                Headers["ContentType"] = value;
            }
        }
    }
}