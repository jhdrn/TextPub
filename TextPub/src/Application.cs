using TextPub.Collections;
using TextPub.Models;

namespace TextPub
{
    public static class Application
    {
        private static PostCollection _posts;
        private static PageCollection _pages;
        private static SnippetCollection _snippets;

        private static Configuration _configuration = new Configuration();

        public static Configuration Configuration
        {
            get
            {
                return _configuration; 
            }
        }

        /// <summary>
        /// Returns a collection of posts. By default the posts is sorted by publish date (descending).
        /// </summary>
        /// <returns></returns>
        public static IPostCollection Posts
        {
            get
            {
                if (_posts == null)
                {
                    _posts = new PostCollection(Configuration.PostsPath);
                }
                return _posts;
            }
        }
                
        /// <summary>
        /// Returns all pages.
        /// </summary>
        /// <returns></returns>
        public static IModelCollection<Page> Pages
        {
            get
            {
                if (_pages == null)
                {
                    _pages = new PageCollection(Configuration.PagesPath);
                }
                return _pages;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public static IModelCollection<Snippet> Snippets
        {
            get
            {
                if (_snippets == null)
                {
                    _snippets = new SnippetCollection(Configuration.SnippetsPath);
                }
                return _snippets;
            }
        }

        //internal static void ClearCaches()
        //{
        //    _posts.ClearCache();
        //    _pages.ClearCache();
        //    _snippets.ClearCache();
        //}
    }
}
