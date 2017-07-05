using System.Collections.ObjectModel;
using System.Linq;

namespace Dao.Net.Web {
    public class HttpHeaderCollection : Collection<HttpHeader> {
        public void Add(string name, string value) {

        }

        public string this[string name]
        {
            get
            {
                return this.Where(x => x.Name == name).FirstOrDefault()?.Value;
            }
            set
            {
                var header = this.Where(x => x.Name == name).FirstOrDefault();

                if (header == null) {
                    this.Add(name,value);
                }
                header.Value = value;
            }
        }
    }
}