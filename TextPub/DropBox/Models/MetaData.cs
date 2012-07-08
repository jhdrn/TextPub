using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextPub.DropBox.Models
{
    public class MetaData
    {
        //public string Size { get; set; }
        //public long Bytes { get; set; }
        public string Path { get; set; }
        public bool Is_Dir { get; set; }
        public bool Is_Deleted { get; set; }
        //public string Rev { get; set; }
        //public string Hash { get; set; }
        //public bool Thumb_Exists { get; set; }
        //public string Icon { get; set; }
        public string Modified { get; set; }
        //public string Root { get; set; }
        //public int Revision { get; set; }
        public List<MetaData> Contents { get; set; }

        public DateTime DateModifiedUTC
        {
            get
            {
                return DateTime.SpecifyKind(DateTime.Parse(Modified), DateTimeKind.Utc);
            }
        }

        public string Name
        {
            get
            {
                int lastSlashIndex = Path.LastIndexOf("/");

                if (string.IsNullOrEmpty(Path) || lastSlashIndex == -1)
                {
                    return string.Empty;
                }

                return Path.Substring(lastSlashIndex + 1);
            }
        }

        public string Extension
        {
            get
            {
                int dotIndex = Path.LastIndexOf(".");

                if (Is_Dir || string.IsNullOrEmpty(Path) || dotIndex == -1)
                {
                    return string.Empty;
                }

                return Path.Substring(dotIndex);
            }
        }
    }
}
