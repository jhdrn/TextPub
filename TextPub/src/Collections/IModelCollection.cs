using System.Collections.Generic;

namespace TextPub.Collections
{
    public interface IModelCollection<T> : IEnumerable<T>
    {
        T this[string id] { get; }
    }
}
