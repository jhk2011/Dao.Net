using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;


namespace Dao.Net {

    public class Packet {

        public static readonly byte[] EmptyBuffer = new byte[0];

        public Packet(int type) {
            _type = type;
        }

        public Packet() {

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

        public void SetObject(object obj) {

            XmlSerializer ser = new XmlSerializer(obj.GetType());

            var ms = new MemoryStream();

            ser.Serialize(ms, obj);

            Buffer = ms.ToArray();

            //Console.WriteLine(GetString());

            //BinaryFormatter formatter = new BinaryFormatter();
            //var ms = new MemoryStream();
            //formatter.Serialize(ms, obj);
            //Buffer = ms.ToArray();
        }

        public T GetObject<T>() {

            //Console.WriteLine(GetString());

            XmlSerializer ser = new XmlSerializer(typeof(T));

            var ms = new MemoryStream(Buffer);

            return (T)ser.Deserialize(ms);

            //BinaryFormatter formatter = new BinaryFormatter();
            //var ms = new MemoryStream(Buffer);
            //return (T)formatter.Deserialize(ms);
        }
    }

}
