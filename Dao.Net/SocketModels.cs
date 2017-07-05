using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Net {

    public interface IRequest {
        Guid Id { get; set; }
    }

    public interface ITransferable : IRequest {
        string SrcUserId { get; set; }

        string DestUserId { get; set; }
    }

    public interface IResponse {
        Guid Id { get; set; }
        bool Success { get; set; }
        string Message { get; set; }
    }

    [Serializable]
    public class Response : IResponse {
        public Guid Id { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
