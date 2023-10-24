using MFilesAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base64ApiClient.model
{
    public class MFObjectFile
    {

        public MFObjectFile(ObjectFile objectFile)
        {
            this.FileGUID = objectFile.FileGUID;
            this.Extension = objectFile.Extension;
            this.Title = objectFile.Title;
            this.ID = objectFile.ID;
            this.Version = objectFile.Version;
            this.ChangeTimeUtc = objectFile.ChangeTimeUtc;
            this.NameForFileSystem = objectFile.GetNameForFileSystem();
            this.LogicalSize = objectFile.LogicalSize;
        }
        public string FileGUID { get; private set; }

        public string Extension { get; private set; }

        public string Title { get; private set; }

        public int ID { get; private set; }

        public int Version { get; set; }

        public DateTime ChangeTimeUtc { get; private set; }

        public string NameForFileSystem { get; private set; }

        public long LogicalSize { get; private set; }
    }
}
