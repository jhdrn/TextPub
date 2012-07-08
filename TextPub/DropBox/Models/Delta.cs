using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextPub.DropBox.Models
{
    public class Delta
    {

        public string Cursor { get; set; }

        public IList<DeltaEntry> Entries { get; set; }

        public bool Reset { get; set; }

        public bool Has_More { get; set; }
    }
}
