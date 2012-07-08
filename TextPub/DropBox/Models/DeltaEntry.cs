using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextPub.DropBox.Models
{
    public class DeltaEntry
    {
        public string Path { get; set; }
        public MetaData MetaData { get; set; }
    }
}
