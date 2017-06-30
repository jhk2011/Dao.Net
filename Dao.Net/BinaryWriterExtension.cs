using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Dao.Net {
    public static class BinaryWriterExtension {

        public static void Write(this BinaryWriter writer, Guid guid) {
            writer.Write(guid.ToByteArray());
        }

        public static void WriteSection(this BinaryWriter writer, byte[] buffer) {
            if (buffer == null || buffer.Length == 0) {
                writer.Write(0);
            } else {
                writer.Write(buffer.Length);
                writer.Write(buffer);
            }
        }

        public static void WriteSection(this BinaryWriter writer, string s, Encoding encoding) {
            byte[] buffer = null;
            if (!String.IsNullOrEmpty(s)) {
                buffer = encoding.GetBytes(s);
            }
            WriteSection(writer, buffer);
        }

        public static void WriteSection(this BinaryWriter writer, string s) {
            WriteSection(writer, s, Encoding.UTF8);
        }

        public static void WriteSection(this BinaryWriter writer, int value) {
            writer.Write(4);
            writer.Write(value);
        }

        public static void WriteSection(this BinaryWriter writer, Guid guid) {
            WriteSection(writer, guid.ToByteArray());
        }

        public static void WriteSection(this BinaryWriter writer, long value) {
            writer.Write(8);
            writer.Write(value);
        }

        public static void WriteSection(this BinaryWriter writer, DateTime value) {
            long value2 = value.ToBinary();
            WriteSection(writer, value2);
        }

        public static void WriteSection(this BinaryWriter writer, bool value) {
            writer.Write(1);
            writer.Write(value);
        }

        //public static void WriteSection(this BinaryWriter writer, IEnumerable<TransferableObject> items) {
        //    if (items == null || items.Count() == 0) {
        //        writer.Write(0);
        //    } else {
        //        writer.Write(items.Count());
        //        foreach (var item in items) {
        //            byte[] buffer = item.GetBuffer();
        //            writer.WriteSection(buffer);
        //        }
        //    }
        //}

    }
}
