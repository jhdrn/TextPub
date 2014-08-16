using System.Collections.Generic;
using System.Linq;
using TextPub.Models;

namespace TextPub.Collections
{
    internal class ModelCollection<T> : List<T>, IModelCollection<T> where T : IIdentity
    {
        internal ModelCollection(IEnumerable<T> models)
            : base(models)
        {
        }

        public T this[string id]
        {
            get
            {
                return this.SingleOrDefault(m => m.Id == id);
            }
        }
    }
}
