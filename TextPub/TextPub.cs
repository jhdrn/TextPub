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
using TextPub.DropBox;

namespace TextPub
{
    public static class TextPub
    {
        private static ArticleRepository _articleRepository;
        private static PageRepository _pageRepository;
        private static SnippetRepository _snippetRepository;

        internal static Configuration Configuration = new Configuration();
       
        internal static ArticleRepository ArticleRepository
        {
            get
            {
                if (_articleRepository == null)
                {
                    _articleRepository = new ArticleRepository(Configuration.ArticlesPath);
                }
                return _articleRepository;
            }
        }

        internal static PageRepository PageRepository
        {
            get
            {
                if (_pageRepository == null)
                {
                    _pageRepository = new PageRepository(Configuration.PagesPath);
                }
                return _pageRepository;
            }
        }

        internal static SnippetRepository SnippetRepository
        {
            get
            {
                if (_snippetRepository == null)
                {
                    _snippetRepository = new SnippetRepository(Configuration.SnippetsPath);
                }
                return _snippetRepository;
            }
        }

        /// <summary>
        /// Returns all articles.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ArticleModel> Articles()
        {
            return Articles(categoryId: null, limit: null, page: null, sortDirections: null, sortBys: null);
        }

        public static IEnumerable<ArticleModel> Articles(string sortBy, SortDirection? sortDirection, int? limit, int? page)
        {
            return Articles(null, new string[] { sortBy }, sortDirection.HasValue ? new SortDirection[] { sortDirection.Value } : null, limit, page);
        }

        public static IEnumerable<ArticleModel> Articles(string categoryId, int? limit, int? page)
        {
            return Articles(categoryId, null, null, limit, page);
        }

        public static IEnumerable<ArticleModel> Articles(string categoryId, string sortBy, SortDirection sortDirection, int? limit, int? page)
        {
            return Articles(categoryId, new string[] { sortBy }, new SortDirection[] { sortDirection }, limit, page);
        }

        /// <summary>
        /// Returns a collection of articles. By default the articles is sorted by publish date (descending).
        /// </summary>
        /// <param name="categoryId">Filter the result set to articles in the category with the supplied categoryId.</param>
        /// <param name="sortBys">An array of the names of the article properties that the result set should be sorted on.</param>
        /// <param name="sortDirections">An array of SortDirection's. Should map to the array of sortBy's.</param>
        /// <param name="limit">How many articles should be returned? If this parameter is passed, the returned type is a StaticPagedList<ArticleModel> which contains additional information for pagination.</param>
        /// <param name="page">Used to offset the limit parameter. Passing a value of 1/0/null will return the first 10 articles, passing a value of 2 will return article 11 to 20, etc.</param>
        /// <returns></returns>
        public static IEnumerable<ArticleModel> Articles(string categoryId, string[] sortBys, SortDirection[] sortDirections, int? limit, int? page)
        {
            var articles = (IEnumerable<ArticleModel>)ArticleRepository.GetList();

            if (!string.IsNullOrWhiteSpace(categoryId))
            {
                articles = articles.Where(a => a.Category != null && a.Category.Id == categoryId);
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

        /// <summary>
        /// Returns the article with the supplied id or null if no article with that id exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ArticleModel Article(string id)
        {
            return ArticleRepository.Get(id);
        }

        /// <summary>
        /// Returns all article categories.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<CategoryModel> ArticleCategories()
        {
            return ArticleRepository.GetCategories();
        }

        /// <summary>
        /// Returns the article category with the supplied id or null if no article category with that id exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CategoryModel ArticleCategory(string id)
        {
            return ArticleCategories().FirstOrDefault(c => c.Id == id);
        }

        /// <summary>
        /// Returns the page with the supplied id or null if no page with that id exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static PageModel Page(string id)
        {
            return PageRepository.Get(id);
        }

        /// <summary>
        /// Returns all pages.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<PageModel> Pages()
        {
            return PageRepository.GetList();
        }

        /// <summary>
        /// Returns the snippet with the supplied id or null if no snippet with that id exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static SnippetModel Snippet(string id)
        {
            return SnippetRepository.Get(id);
        }


        /// <summary>
        /// Indicates whether TextPub is fully configured or not.
        /// </summary>
        public static bool IsConfigured { 
            get {

                return !string.IsNullOrWhiteSpace(Configuration.DropBoxConsumerSecret)
                    && !string.IsNullOrWhiteSpace(Configuration.DropBoxConsumerToken)
                    && !string.IsNullOrWhiteSpace(Configuration.DropBoxUserSecret)
                    && !string.IsNullOrWhiteSpace(Configuration.DropBoxUserToken);
            }
        }


        public static void ForceDropBoxSynchronization()
        {

            // TODO
            //var job = new DropBoxSyncJob(new TimeSpan(), new TimeSpan(), Configuration);
            //job.Execute();
            
        }

        /// <summary>
        /// Wrapper method to save the configuration. 
        /// </summary>
        /// <param name="consumerToken"></param>
        /// <param name="consumerSecret"></param>
        /// <param name="userToken"></param>
        /// <param name="userSecret"></param>
        /// <param name="syncInterval"></param>
        public static void Configure(string consumerToken, string consumerSecret, string userToken, string userSecret, int syncInterval)
        {
            Configuration.DropBoxConsumerToken = consumerToken;
            Configuration.DropBoxConsumerSecret = consumerSecret;

            Configuration.DropBoxUserToken = userToken;
            Configuration.DropBoxUserSecret = userSecret;

            Configuration.DropBoxSyncInterval = syncInterval;

            Configuration.Save();
        }

        private static object GetPropertyValue(object obj, string propertyName)
        {
            if (obj == null)
            {
                return null;
            }
            return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }


        internal static void ClearCaches()
        {
            ArticleRepository.RemoveAll();
            PageRepository.RemoveAll();
            SnippetRepository.RemoveAll();
        }
    }
}
