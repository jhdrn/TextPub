using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.IO;
using TextPub.Models;

namespace TextPub.Collections
{
    internal class SnippetCollection : CachedModelCollection<Snippet>
    {
        public SnippetCollection(string path) 
            : base(path) 
        { 
        }

        protected override Snippet CreateModel(FileInfo fileInfo, string relativePath)
        {
            var id = GenerateId(fileInfo, relativePath);
            var path = GenerateLocalPath(relativePath, fileInfo.Name);

            byte[] fileContents = File.ReadAllBytes(fileInfo.FullName);
            string fileContentsString = System.Text.Encoding.UTF8.GetString(fileContents).TrimStart();
            
            return new Snippet(id, path, MarkdownHelper.Transform(fileContentsString));
        }

    }
}