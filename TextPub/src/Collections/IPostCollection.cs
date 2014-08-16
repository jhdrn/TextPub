using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TextPub.Models;

namespace TextPub.Collections
{
    public interface IPostCollection : IModelCollection<Post>
    {
        IModelCollection<Category> Categories { get; }
    }
}
