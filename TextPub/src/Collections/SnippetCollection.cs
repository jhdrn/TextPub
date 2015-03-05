using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using TextPub.Models;
using System.Text;

namespace TextPub.Collections
{
    internal class SnippetCollection : CachedModelCollection<ISnippet>
    {
        public SnippetCollection(string path, Func<ISnippet, ISnippet> decoratorProvider)
            : base(path, decoratorProvider) 
        { 
        }

        protected override ISnippet CreateModel(FileInfo fileInfo, string relativePath, string path, string id, string html)
        {            
            return new Snippet(id, path, html);
        }

    }
}