using System;

namespace TextPub.Models
{
    public class Post : IPost
    {
        internal Post(string id, string path, string title, string body, DateTime? publishDate, Category category)
        {
            Id = id;
            Path = path;
            Title = title;
            Body = body;
            PublishDate = publishDate;
            Category = category;
        }

        public string Id { get; private set; }

        public string Path { get; private set; }

        public string Title { get; private set;  }

        public string Body { get; private set; }

        public DateTime? PublishDate { get; private set; }

        public Category Category { get; private set; }
    }
}
