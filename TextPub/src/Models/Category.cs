using System.Collections.Generic;

namespace TextPub.Models
{
    public class Category : IIdentity
    {
        internal Category(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; private set; }

        public string Name { get; private set; }

        public Category Parent { get; internal set; }

        public IEnumerable<Category> Children { get; internal set; }
    }
}
