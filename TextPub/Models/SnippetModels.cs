using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.IO;

namespace TextPub.Models
{
    public class SnippetModel : BaseModel
    {
        public string Content { get; set; }
    }

    internal class SnippetRepository : CacheRepository<SnippetModel>
    {
        public SnippetRepository(string path) : base(path) { }

        protected override SnippetModel CreateModel(FileInfo fileInfo, string relativePath)
        {
            var model = new SnippetModel
            {
                Id = GenerateId(fileInfo, relativePath),
                Path = GenerateLocalPath(relativePath, fileInfo.Name)
            };

            byte[] fileContents = File.ReadAllBytes(fileInfo.FullName);
            string fileContentsString = System.Text.Encoding.UTF8.GetString(fileContents).TrimStart();

            model.Content = MarkdownHelper.Transform(fileContentsString);

            return model;
        }

    }
}