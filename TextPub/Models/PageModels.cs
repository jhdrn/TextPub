using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Text.RegularExpressions;
using System.IO;

namespace TextPub.Models
{
    public class PageModel : BaseModel
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public int Level { get; set; }
        public PageModel Parent { get; set; }
        public IEnumerable<PageModel> Children { get; set; }
        public int? SortOrder { get; set; }
    }

    internal class PageRepository : CacheRepository<PageModel>
    {
        private Regex _htmlHeadingRegex = new Regex(@"^\s*<h(?<HeadingLevel>\d)[^>]*?>(?<Title>.*)(?<SortOrder>\[[0-9]+\])?</h\k<HeadingLevel>>");

        public PageRepository(string path) : base(path) { }

        protected override PageModel CreateModel(FileInfo fileInfo, string relativePath)
        {

            var model = new PageModel
            {
                Id = GenerateId(fileInfo, relativePath),
                Path = GenerateLocalPath(relativePath, fileInfo.Name),
            };

            byte[] fileContents = File.ReadAllBytes(fileInfo.FullName);

            string fileContentsString = System.Text.Encoding.UTF8.GetString(fileContents).TrimStart();

            var html = MarkdownHelper.Transform(fileContentsString);
            var match = _htmlHeadingRegex.Match(html);
            if (match.Success)
            {
                model.Title = match.Groups["Title"].Value;

                string sortOrder = match.Groups["SortOrder"].Value;
                if (!string.IsNullOrWhiteSpace(sortOrder))
                {
                    try
                    {
                        model.SortOrder = Convert.ToInt32(sortOrder);
                    }
                    catch { };
                }

                html = html.Substring(match.Index + match.Length);
            }

            model.Body = html;

            if (string.IsNullOrWhiteSpace(model.Title))
            {
                model.Title = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
            }

            model.Level = model.Path.Count(p => p == '/');

            return model;
        }

        protected override void RefreshList()
        {
            base.RefreshList();

            // Set page parents
            IEnumerable<PageModel> list = GetList().OrderBy(p => p.Path.Length);

            foreach (PageModel page in list)
            {
                if (page.Level > 0)
                {
                    string parentId = page.Id.Substring(0, page.Id.LastIndexOf('/'));

                    page.Parent = list.FirstOrDefault(p => p.Id == parentId);
                }
            }

            // Set page children
            foreach (PageModel page in list)
            {
                page.Children = list.Where(p => p.Parent != null && p.Parent.Id == page.Id);
            }
        }
    }

}