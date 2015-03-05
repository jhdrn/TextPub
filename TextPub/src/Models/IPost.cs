using System;
namespace TextPub.Models
{
    public interface IPost : IIdentity
    {
        string Body { get; }
        Category Category { get; }
        string Path { get; }
        DateTime? PublishDate { get; }
        string Title { get; }
    }
}
