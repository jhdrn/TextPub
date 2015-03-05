using System;
using System.Collections.Generic;

namespace TextPub.Models
{
    public interface IPage : IIdentity
    {
        string Body { get; }
        IEnumerable<IPage> Children { get; set; }
        int Level { get; }
        IPage Parent { get; set; }
        string Path { get; }
        int? SortOrder { get; }
        string Title { get; }
    }
}
