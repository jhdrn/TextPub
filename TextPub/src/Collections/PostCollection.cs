using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Web.Hosting;
using TextPub.Models;

namespace TextPub.Collections
{
    internal class PostCollection : CachedModelCollection<Post>, IPostCollection
    {
        private Regex _htmlHeadingRegex = new Regex(@"^\s*<h(?<HeadingLevel>\d)[^>]*?>(?<Title>.*)(?<PublishDate>\[[0-9/\-]+\])?</h\k<HeadingLevel>>");

        private IModelCollection<Category> _categories;

        public PostCollection(string path) : base(path) { }

        protected override Post CreateModel(FileInfo fileInfo, string relativePath)
        {
            var id = GenerateId(fileInfo, relativePath);
            var path = GenerateLocalPath(relativePath, fileInfo.Name);
            string title = null;
            var publishDate = fileInfo.LastWriteTimeUtc;
            Category category = null;

            string categoryName = fileInfo.Directory.Name;
            if (categoryName != _relativeFilesPath)
            {
                var categoryId = relativePath.Substring(_relativeFilesPath.Length).Replace('\\', '/').TrimStart('/').UrlFriendly();
                category = new Category(categoryId, categoryName);
            }

            byte[] fileContents = File.ReadAllBytes(fileInfo.FullName);

            string fileContentsString = System.Text.Encoding.UTF8.GetString(fileContents).TrimStart();

            var html = MarkdownHelper.Transform(HttpUtility.HtmlEncode(fileContentsString));

            var match = _htmlHeadingRegex.Match(html);
            if (match.Success)
            {
                title = match.Groups["Title"].Value;
                var publishDateString = match.Groups["PublishDate"].Value;
                if (!string.IsNullOrWhiteSpace(publishDateString))
                {
                    try
                    {
                        publishDate = DateTime.Parse(publishDateString);
                    }
                    catch { }
                }
                html = html.Substring(match.Index + match.Length);
            }


            if (string.IsNullOrWhiteSpace(title))
            {
                title = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
            }

            return new Post(
                id: id,
                path: path,
                title: title,
                body: html,
                publishDate: publishDate,
                category: category
            );
        }

        protected override void RefreshCollection()
        {
            _categories = null;
            base.RefreshCollection();
        }

        public IModelCollection<Category> Categories
        {
            get 
            {
                if (_categories == null)
                {
                    _categories = new ModelCollection<Category>(GetCollection().Select(a => a.Category).Where(c => c != null).GroupBy(c => c.Name).Select(g => g.First()));
                }
                return _categories;
            }
        }

    }
}