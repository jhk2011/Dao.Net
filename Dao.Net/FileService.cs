using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dao.Net {
    [Serializable]
    public class FileItem {
        public string Name { get; set; }
        public string FullName { get; set; }

        public bool IsFile { get; set; }
    }
    public interface IFileService {
        byte[] Download(string path);
        void Upload(string path, byte[] bytes);

        List<FileItem> GetFileItem(string path);

        void Delete(string path);

        void CreateFile(string path);

        void CreateFolder(string path);
    }
    public class FileService : IFileService {

        public void CreateFile(string path) {
            throw new NotImplementedException();
        }

        public void CreateFolder(string path) {
            throw new NotImplementedException();
        }

        public void Delete(string path) {
            throw new NotImplementedException();
        }

        public byte[] Download(string path) {
            throw new NotImplementedException();
        }

        public List<FileItem> GetFileItem(string path) {
            if (path == null || path == "") {
                return DriveInfo.GetDrives()
                    .Select(x => new FileItem {
                        Name = x.Name,
                        IsFile = false,
                        FullName = x.RootDirectory.FullName
                    })
                    .ToList();
            }
            if (Directory.Exists(path)) {
                return Directory.GetDirectories(path)
                    .Select(x => new FileItem {
                        Name = x,
                        FullName=x,
                        IsFile = true
                    })
                    .Concat(Directory.GetFiles(path).Select(x => new FileItem {
                        Name = x,
                        FullName=x
                    }))
                    .ToList();
            }
            return null;
        }

        public void Upload(string path, byte[] bytes) {
            throw new NotImplementedException();
        }
    }
}
