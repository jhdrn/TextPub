using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TextPub.Models;

namespace TextPub.Collections
{
    internal class PageCollection : CachedModelCollection<IPage>
    {
        private Regex _htmlHeadingRegex = new Regex(@"^\s*<h(?<HeadingLevel>\d)[^>]*?>(?(.*\[\d+\])(?<Title>.*)\[(?<SortOrder>\d+)\]|(?<Title>.+))?</h\k<HeadingLevel>>");
        
        public PageCollection(string path, MarkdownOptions options, Func<IPage, IPage> decoratorProvider)
            : base(path, options, decoratorProvider) 
        {
        }

        protected override IPage CreateModel(FileInfo fileInfo, string relativePath, string path, string id, string html)
        {
            int? sortOrder = null;
            string title = null;

            var match = _htmlHeadingRegex.Match(html);
            if (match.Success)
            {
                title = match.Groups["Title"].Value;

                string sortOrderString = match.Groups["SortOrder"].Value;
                if (!string.IsNullOrWhiteSpace(sortOrderString))
                {
                    try
                    {
                        sortOrder = Convert.ToInt32(sortOrderString);
                    }
                    catch { };
                }

                html = html.Substring(match.Index + match.Length);
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                title = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
            }

            return new Page(
                id: id, 
                path: path, 
                title: title.Trim(), 
                body: html,
                level: path.Count(p => p == Path.DirectorySeparatorChar), 
                sortOrder: sortOrder
            );
        }

        protected override void RebuildCache()
        {
            var list = ReadFilesRecursively(_filesPath).OrderBy(p => p.Path.Length).ToList();

            // Set page parents
            foreach (IPage page in list)
            {
                if (page.Level > 0)
                {
                    string parentId = page.Id.Substring(0, page.Id.LastIndexOf('/'));

                    page.Parent = list.FirstOrDefault(p => p.Id == parentId);
                }
            }

            // Set page children
            foreach (IPage page in list)
            {
                page.Children = list.Where(p => p.Parent != null && p.Parent.Id == page.Id);
            }

            InsertIntoCache(list.OrderBy(p => p.SortOrder).ToList());
        }
    }
}
