using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Net {

    [Serializable]
    public class BreakException : Exception {
        public BreakException() { }

        public BreakException(string message) : base(message) { }
        public BreakException(string message, Exception inner) : base(message, inner) { }
        protected BreakException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
