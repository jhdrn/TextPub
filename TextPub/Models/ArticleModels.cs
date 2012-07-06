using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Web.Hosting;

namespace TextPub.Models
{
    public class ArticleModel : BaseModel
    {
        public string Title { get; set; }

        public string Body { get; set; }

        public DateTime? PublishDate { get; set; }
        
        public CategoryModel Category { get; set; }
    }

    public class CategoryModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public CategoryModel Parent { get; set; }
        public IEnumerable<CategoryModel> Children { get; set; }
    }

    internal class ArticleRepository : CacheRepository<ArticleModel>
    {
        private Regex _htmlHeadingRegex = new Regex(@"^\s*<h(?<HeadingLevel>\d)[^>]*?>(?<Title>.*)(?<PublishDate>\[[0-9/\-]+\])?</h\k<HeadingLevel>>");

        private IEnumerable<CategoryModel> _categories;

        public override string RelativeFilesPath
        {
            get { 
                return "articles";
            }
        }

        protected override ArticleModel CreateModel(FileInfo fileInfo, string relativePath)
        {
            
            var model = new ArticleModel
            {
                Id = GenerateId(fileInfo, relativePath),
                Path = GenerateLocalPath(relativePath, fileInfo.Name),
                PublishDate = fileInfo.LastWriteTimeUtc
            };

            string categoryName = fileInfo.Directory.Name;
            if (categoryName != RelativeFilesPath)
            {
                model.Category = new CategoryModel
                {
                    Id = relativePath.Substring(RelativeFilesPath.Length).Replace('\\', '/').TrimStart('/').UrlFriendly(),
                    Name = categoryName
                };
            }

            byte[] fileContents = File.ReadAllBytes(fileInfo.FullName);


            string fileContentsString = System.Text.Encoding.UTF8.GetString(fileContents).TrimStart();
            
            var html = MarkdownHelper.Transform(fileContentsString);

            var match = _htmlHeadingRegex.Match(html);
            if (match.Success)
            {
                model.Title = match.Groups["Title"].Value;
                var publishDateString = match.Groups["PublishDate"].Value;
                if (!string.IsNullOrWhiteSpace(publishDateString))
                {
                    try
                    {
                        model.PublishDate = DateTime.Parse(publishDateString);
                    }
                    catch { }
                }
                html = html.Substring(match.Index + match.Length);
            }

            model.Body = html;

            if (string.IsNullOrWhiteSpace(model.Title))
            {
                model.Title = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
            }

            return model;
        }

        public IEnumerable<CategoryModel> GetCategories()
        {
            if (_categories == null)
            {
                _categories = GetList().Select(a => a.Category).Where(c => c != null).GroupBy(c => c.Name).Select(g => g.First());
            }
            return _categories;
        }

        protected override void RefreshList()
        {
            _categories = null;
            base.RefreshList();
        }
    }
}