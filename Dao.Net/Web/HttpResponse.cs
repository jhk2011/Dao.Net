using System;
using System.Collections.Generic;

namespace Dao.Net.Web {


    public class HttpResponse {
        public string Line { get; set; }
        public HttpHeaderCollection Headers { get; set; }
        public byte[] Body { get; set; }

        public HttpResponse() {
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