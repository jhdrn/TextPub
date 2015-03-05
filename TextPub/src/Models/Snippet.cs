
namespace TextPub.Models
{
    public class Snippet : ISnippet
    {
        internal Snippet(string id, string path, string content)
        {
            Id = id;
            Path = path;
            Content = content;
        }

        public string Id { get; private set; }

        public string Path { get; private set; }

        public string Content { get; private set; }
    }
}
