using System;
using System.IO;
using System.Text;
using System.Linq;

namespace Dao.Net {

    public class FileInfo {
        public string Name { get; set; }
        public string FullName { get; set; }
        public int Size { get; set; }
        public bool Directory { get; set; }
    }

    public class FileManager : ISocketHandler {

        SocketSession _session;
        public FileManager(SocketSession session) {
            _session = session;
        }

        public void GetFilesAsync(string path) {
            Packet packet = new Packet(FilePackets.GetFiles);
            packet.SetString(path);

            _session.SendAsync(packet);
        }

        private SocketSession FindSession(string userid) {
            throw new NotImplementedException();
        }

        public void Handle(Packet packet, SocketSession session) {
            if (packet.Type == FilePackets.GetFiles) {
                string path = Encoding.UTF8.GetString(packet.Buffer);
                string[] files = GetFiles(path);
                string s = string.Join("\r\n", files);
                Packet p2 = new Packet(FilePackets.GetFilesReply);
                p2.SetString(s);
                session.SendAsync(p2);
            } else if (packet.Type == FilePackets.GetFilesReply) {
                string[] files = packet.GetString()
                   .Split(new string[] { "\r\n" },
                   StringSplitOptions.None);

                GetFilesComplete?.Invoke(files);
            }
        }

        protected virtual string[] GetFiles(string path) {
            if (string.IsNullOrEmpty( path )) {
                return DriveInfo.GetDrives().Select(x => x.Name)
                      .ToArray();
            }
            return Directory.GetFileSystemEntries(path);
        }

        public event Action<string[]> GetFilesComplete;


    }
}
