using System;
namespace TextPub.Models
{
    public interface ISnippet : IIdentity
    {
        string Content { get; }
        string Path { get; }
    }
}
