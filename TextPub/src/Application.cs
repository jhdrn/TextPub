using System;
using System.Web.Hosting;
using TextPub.Collections;
using TextPub.Models;

namespace TextPub
{
    public sealed class Application
    {
        private static Lazy<PostCollection> _posts = new Lazy<PostCollection>(() => new PostCollection(GetAbsolutePath(Configuration.PostsPath)));
        private static Lazy<PageCollection> _pages = new Lazy<PageCollection>(() => new PageCollection(GetAbsolutePath(Configuration.PagesPath)));
        private static Lazy<SnippetCollection> _snippets = new Lazy<SnippetCollection>(() => new SnippetCollection(GetAbsolutePath(Configuration.SnippetsPath)));

        private static Configuration _configuration = new Configuration();
        private static Application _instance = new Application();

        private Application()
        {
        }

        //public static Application Instance 
        //{ 
        //    get {
        //        return _instance;
        //    }
        //}
        
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
                return _posts.Value;
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
                return _pages.Value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public static IModelCollection<Snippet> Snippets
        {
            get
            {
                return _snippets.Value;
            }
        }

        public static void ClearCaches()
        {
            if (_posts.IsValueCreated)
                _posts.Value.ClearCache();

            if (_pages.IsValueCreated)
                _pages.Value.ClearCache();

            if (_snippets.IsValueCreated)
                _snippets.Value.ClearCache();
        }

        private static string GetAbsolutePath(string path)
        {
            var basePath = Configuration.BasePath;
            if (basePath.StartsWith("~"))
            {
                basePath = HostingEnvironment.MapPath(basePath);
            }

            return basePath + "/" + path;
        }
    }
}
