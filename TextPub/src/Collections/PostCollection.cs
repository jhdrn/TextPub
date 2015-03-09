using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using TextPub.Models;

namespace TextPub.Collections
{
    internal class PostCollection : CachedModelCollection<IPost>, IPostCollection
    {
        private Regex _htmlHeadingRegex = new Regex(@"^\s*<h(?<HeadingLevel>\d)[^>]*?>(?(.*\[[\d/\-]+\])(?<Title>.*)\[(?<PublishDate>[\d/\-]+)\]|(?<Title>.+))?</h\k<HeadingLevel>>");

        private IModelCollection<Category> _categories;

        public PostCollection(string path, Func<IPost, IPost> decoratorProvider)
            : base(path, decoratorProvider) 
        {
        }

        protected override IPost CreateModel(FileInfo fileInfo, string relativePath, string path, string id, string html)
        {
            string title = null;
            var publishDate = fileInfo.LastWriteTimeUtc;
            Category category = null;

            string categoryName = fileInfo.Directory.Name;
            if (categoryName != _filesPath)
            {
                var categoryId = relativePath.Substring(_filesPath.Length).Replace('\\', '/').TrimStart('/').UrlFriendly();
                category = new Category(categoryId, categoryName);
            }

            var match = _htmlHeadingRegex.Match(html);
            if (match.Success)
            {
                title = match.Groups["Title"].Value.Trim();
                var publishDateString = match.Groups["PublishDate"].Value.Trim();
                if (!string.IsNullOrWhiteSpace(publishDateString))
                {
                    DateTime.TryParse(publishDateString, out publishDate);
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
                title: title.Trim(),
                body: html,
                publishDate: publishDate,
                category: category
            );
        }

        protected override void RebuildCache()
        {
            _categories = null;
            base.RebuildCache();
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