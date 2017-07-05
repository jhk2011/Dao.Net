namespace Dao.Net.Web {
    public class HttpHeader {
        public HttpHeader() {

        }

        public HttpHeader(string name, string value) {
            this.Name = name;
            this.Value = value;
        }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}