using System;

namespace Dao.Net {
    public interface ISocketHandler {
        void Handle(HandleContext context);
        void Close(HandleContext context);
        void Accept(HandleContext context);

        void Send(HandleContext context);
    }

    public class SocketHandler : ISocketHandler {
        public virtual void Handle(HandleContext context) {
            
        }

        public virtual void Close(HandleContext context) {

        }

        public virtual void Accept(HandleContext context) {

        }

        public virtual void Send(HandleContext context) {
            
        }
    }
}
