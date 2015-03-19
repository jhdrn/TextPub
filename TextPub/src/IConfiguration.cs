using System;
using TextPub.Models;

namespace TextPub
{
    public interface IConfiguration
    {
        string BasePath { get; set; }
        string PagesPath { get; set; }
        string PostsPath { get; set; }
        string SnippetsPath { get; set; }

        MarkdownOptions MarkdownOptions { get; }

        Func<IPage, IPage> PageDecoratorProvider { get; set; }
        Func<IPost, IPost> PostDecoratorProvider { get; set; }
        Func<ISnippet, ISnippet> SnippetDecoratorProvider { get; set; }
    }
}
