using System.Collections.Generic;

namespace TextPub.Models
{
    public class Page : IPage
    {
        internal Page(string id, string path, string title, string body, int level, int? sortOrder)
        {
            Id = id;
            Path = path;
            Title = title;
            Body = body;
            Level = level;
            SortOrder = sortOrder;
        }

        public string Id { get; private set; }

        public string Path { get; private set; }

        public string Title { get; private set;  }

        public string Body { get; private set; }

        public int Level { get; private set; }

        public IPage Parent { get; set; }

        public IEnumerable<IPage> Children { get; set; }

        public int? SortOrder { get; private set; }
    }
}