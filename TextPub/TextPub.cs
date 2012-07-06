using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TextPub.Models;
using System.Web.Helpers;
using System.Linq.Expressions;
using System.Web.Hosting;
using System.Xml;
using PagedList;

namespace TextPub
{
    public static class TextPub
    {
        public const string DropBoxConsumerToken = "DropBoxConsumerToken";
        public const string DropBoxConsumerSecret = "DropBoxConsumerSecret";
        public const string DropBoxUserToken = "DropBoxUserToken";
        public const string DropBoxUserSecret = "DropBoxUserSecret";
        public const string DropBoxSyncInterval = "DropBoxSyncInterval";
        internal const string DropBoxDeltaCursor = "DropBoxDeltaCursor";

        private static ArticleRepository _articleRepository = new ArticleRepository();
        private static PageRepository _pageRepository = new PageRepository();
        private static SnippetRepository _snippetRepository = new SnippetRepository();

        internal static ConfigurationManager ConfigurationManager = new ConfigurationManager();

       
        internal static ArticleRepository ArticleRepository
        {
            get
            {
                return _articleRepository;
            }
        }

        internal static PageRepository PageRepository
        {
            get
            {
                return _pageRepository;
            }
        }

        internal static SnippetRepository SnippetRepository
        {
            get
            {
                return _snippetRepository;
            }
        }

        public static IEnumerable<ArticleModel> Articles()
        {
            return Articles(category: null, limit: null, page: null, sortDirections: null, sortBys: null);
        }

        public static IEnumerable<ArticleModel> Articles(string sortBy, SortDirection? sortDirection, int? limit, int? page)
        {
            return Articles(null, new string[] { sortBy }, sortDirection.HasValue ? new SortDirection[] { sortDirection.Value } : null, limit, page);
        }

        public static IEnumerable<ArticleModel> Articles(string category, int? limit, int? page)
        {
            return Articles(category, null, null, limit, page);
        }

        public static IEnumerable<ArticleModel> Articles(string category, string sortBy, SortDirection sortDirection, int? limit, int? page)
        {
            return Articles(category, new string[] { sortBy }, new SortDirection[] { sortDirection }, limit, page);
        }

        public static IEnumerable<ArticleModel> Articles(string category, string[] sortBys, SortDirection[] sortDirections, int? limit, int? page)
        {
            var articles = (IEnumerable<ArticleModel>)ArticleRepository.GetList();

            if (!string.IsNullOrWhiteSpace(category))
            {
                articles = articles.Where(a => a.Category != null && a.Category.Name == category);
            }

            if (sortBys != null && sortBys.Where(x => !string.IsNullOrWhiteSpace(x)).Any())
            {
                IOrderedEnumerable<ArticleModel> orderedArticles = null;
                for (int i = 0; i < sortBys.Length; i++)
                {
                    var sortDirection = sortDirections != null ? sortDirections.ElementAtOrDefault(i) : SortDirection.Ascending;

                    if (i == 0)
                    {
                        if (sortDirection == SortDirection.Ascending)
                        {
                            orderedArticles = articles.OrderBy(a => GetPropertyValue(a, sortBys[i]));
                        }
                        else
                        {
                            orderedArticles = articles.OrderByDescending(a => GetPropertyValue(a, sortBys[i]));
                        }
                    }
                    else
                    {
                        if (sortDirection == SortDirection.Ascending)
                        {
                            orderedArticles = orderedArticles.ThenBy(a => GetPropertyValue(a, sortBys[i]));
                        }
                        else
                        {
                            orderedArticles = orderedArticles.ThenByDescending(a => GetPropertyValue(a, sortBys[i]));
                        }
                    }
                }
                articles = orderedArticles;
            }
            else
            {
                // Sort by publish date by default
                articles = articles.OrderByDescending(a => a.PublishDate);
            }

            if (limit != null)
            {
                
                if (page == null || page <= 1)
                {
                    return new StaticPagedList<ArticleModel>(
                        articles.Take((int)limit), 
                        1, 
                        (int)limit, 
                        articles.Count()
                    );
                }
                return new StaticPagedList<ArticleModel>(
                    articles.Skip((int)page - 1 * (int)limit).Take((int)limit), 
                    (int)page, 
                    (int)limit, 
                    articles.Count()
                );
            }
            
            return articles;

        }

        public static ArticleModel Article(string id)
        {
            return ArticleRepository.Get(id);
        }

        public static IEnumerable<CategoryModel> ArticleCategories()
        {
            return ArticleRepository.GetCategories();
        }

        public static CategoryModel ArticleCategory(string id)
        {
            return ArticleCategories().FirstOrDefault(c => c.Id == id);
        }

        public static PageModel Page(string id)
        {
            return PageRepository.Get(id);
        }

        public static IEnumerable<PageModel> Pages()
        {
            return PageRepository.GetList();
        }

        public static SnippetModel Snippet(string id)
        {
            return SnippetRepository.Get(id);
        }


        public static bool IsConfigured { 
            get {

                return !string.IsNullOrWhiteSpace(ConfigurationManager.Get(TextPub.DropBoxConsumerSecret))
                    && !string.IsNullOrWhiteSpace(ConfigurationManager.Get(TextPub.DropBoxConsumerToken))
                    && !string.IsNullOrWhiteSpace(ConfigurationManager.Get(TextPub.DropBoxUserSecret))
                    && !string.IsNullOrWhiteSpace(ConfigurationManager.Get(TextPub.DropBoxUserToken));

                /*
                var appSettings = WebConfigurationManager.OpenWebConfiguration("~/").AppSettings.Settings;

                return appSettings[TextPub.DropBoxConsumerSecret] != null && !string.IsNullOrWhiteSpace(appSettings[TextPub.DropBoxConsumerSecret].Value)
                    && appSettings[TextPub.DropBoxConsumerToken] != null && !string.IsNullOrWhiteSpace(appSettings[TextPub.DropBoxConsumerToken].Value)
                    && appSettings[TextPub.DropBoxUserSecret] != null && !string.IsNullOrWhiteSpace(appSettings[TextPub.DropBoxUserSecret].Value)
                    && appSettings[TextPub.DropBoxUserToken] != null && !string.IsNullOrWhiteSpace(appSettings[TextPub.DropBoxUserToken].Value);
                 */
            }
        }

        public static void Configure(string consumerToken, string consumerSecret, string userToken, string userSecret, int syncInterval)
        {
            
            ConfigurationManager.Put(TextPub.DropBoxConsumerToken, consumerToken);
            ConfigurationManager.Put(TextPub.DropBoxConsumerSecret, consumerSecret);

            ConfigurationManager.Put(TextPub.DropBoxUserToken, userToken);
            ConfigurationManager.Put(TextPub.DropBoxUserSecret, userSecret);

            ConfigurationManager.Put(TextPub.DropBoxSyncInterval, Convert.ToString(syncInterval));
            /*
            var config = WebConfigurationManager.OpenWebConfiguration("~/");
            var appSettings = config.AppSettings.Settings;

            appSettings.Remove(TextPub.DropBoxConsumerToken);
            appSettings.Remove(TextPub.DropBoxConsumerSecret);

            appSettings.Add(TextPub.DropBoxConsumerToken, consumerToken);
            appSettings.Add(TextPub.DropBoxConsumerSecret, consumerSecret);

            appSettings.Remove(TextPub.DropBoxUserToken);
            appSettings.Remove(TextPub.DropBoxUserSecret);

            appSettings.Add(TextPub.DropBoxUserToken, userToken);
            appSettings.Add(TextPub.DropBoxUserSecret, userSecret);

            appSettings.Remove(TextPub.DropBoxSyncInterval);
            appSettings.Add(TextPub.DropBoxSyncInterval, Convert.ToString(syncInterval));

            config.Save(ConfigurationSaveMode.Modified);

            ConfigurationManager.RefreshSection("appSettings");
            */
        }

        private static object GetPropertyValue(object obj, string propertyName)
        {
            if (obj == null)
            {
                return null;
            }
            return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }

    }
}
