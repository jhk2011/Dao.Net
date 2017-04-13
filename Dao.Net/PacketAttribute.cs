using System;


namespace Dao.Net {
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class PacketAttribute : Attribute {
        public int Id { get; set; }
    }

    interface IPacketParser {

    }

}
