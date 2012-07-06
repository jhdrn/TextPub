using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextPub.DropBox
{
    public class Delta
    {

        public string Cursor { get; set; }

        public IList<Entry> Entries { get; set; }

        public bool Reset { get; set; }

        public bool Has_More { get; set; }
    }


    public class Entry
    {
        public string Path { get; set; }
        public MetaData MetaData { get; set; }
    }
}
