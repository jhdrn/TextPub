using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace TextPub.DropBox
{
    internal class DeltaJavaScriptConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            var delta = new Delta();
            delta.Cursor = (string)dictionary["cursor"];
            delta.Has_More = (bool)dictionary["has_more"];
            delta.Reset = (bool)dictionary["reset"];
            delta.Entries = new List<Entry>();

            var entries = dictionary["entries"] as ArrayList;
            if (entries != null)
            {
                foreach (var entry in entries)
                {
                    var entryList = entry as ArrayList;

                    var entryModel = new Entry 
                    {
                        Path = (string)entryList[0]
                    };

                    var metaData = entryList[1];
                    if (metaData != null)
                    {
                        entryModel.MetaData = serializer.ConvertToType<MetaData>(metaData);
                    }

                    delta.Entries.Add(entryModel);

                }
            }

            return delta;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new Type[] { typeof(Delta) }; }
        }
    }
}
