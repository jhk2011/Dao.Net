using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;


namespace Dao.Net {

    public class SysPackets {
        public const int Join = 100;
        public const int Leave = 101;
        public const int List = 102;
    }

    public class UserPackets {
        public const int Join = 2001;
        public const int JoinReply = 2002;
        public const int Leave = 2005;
        public const int LeaveReply = 2005;
        public const int GetUsers = 2003;
        public const int GetUsersReply = 2004;
    }
    public class FilePackets {
        public const int GetFiles = 1001;
        public const int GetFilesReply = 1002;
    }

    public class Packet {

        public static readonly byte[] EmptyBuffer = new byte[0];

        public Guid Id { get; set; }

        public string SrcUserId { get; set; }

        public string DestUserId { get; set; }


        public Packet(int type) {
            _type = type;
            Id = Guid.NewGuid();
        }

        public Packet() {
            Id = Guid.NewGuid();
        }

        int _type;

        public int Type
        {
            get { return _type; }
            set { _type = value; }
        }

        byte[] _buffer;

        public byte[] Buffer
        {
            get { return _buffer ?? EmptyBuffer; }
            set { _buffer = value; }
        }

        public string GetString() {
            return Encoding.UTF8.GetString(Buffer);
        }

        public void SetString(string s) {
            Buffer = Encoding.UTF8.GetBytes(s);
        }

        public T GetObject<T>() {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(Buffer)) {
                return (T)formatter.Deserialize(ms);
            }
        }

        public void SetObject<T>(T obj) {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream()) {
                formatter.Serialize(ms, obj);
                Buffer = ms.ToArray();
            }
        }
    }

}
