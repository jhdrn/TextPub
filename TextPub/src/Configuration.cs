using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Linq;

namespace TextPub
{
    public sealed class Configuration
    {
        private const string _postsPathKey = "PostsPath";
        private const string _pagesPathKey = "PagesPath";
        private const string _snippetsPathKey = "SnippetsPath";

        private System.Configuration.Configuration _configuration;

        internal Configuration()
        {
            // Set some defaults
            PostsPath = PostsPath ?? "posts";
            PagesPath = PagesPath ?? "pages";
            SnippetsPath = SnippetsPath ?? "snippets";
        }

        /// <summary>
        /// The base path of the site powered by TextPub (i.e. /Apps/TextPub/{SitePath}/{PostsPath})
        /// 
        /// Defaults to "posts".
        /// </summary>
        public string PostsPath
        {
            get
            {
                return ConfigurationManager.AppSettings[_postsPathKey];
            }
            set
            {
                WriteSetting(_postsPathKey, value);
            }
        }

        /// <summary>
        /// The base path of the site powered by TextPub (i.e. /Apps/TextPub/{SitePath}/{PagesPath})
        /// 
        /// Defaults to "pages".
        /// </summary>
        public string PagesPath
        {
            get
            {
                return ConfigurationManager.AppSettings[_pagesPathKey];
            }
            set
            {
                WriteSetting(_pagesPathKey, value);
            }
        }

        /// <summary>
        /// The base path of the site powered by TextPub (i.e. /Apps/TextPub/{SitePath}/{SnippetsPath})
        /// 
        /// Defaults to "snippets".
        /// </summary>
        public string SnippetsPath
        {
            get
            {
                return ConfigurationManager.AppSettings[_snippetsPathKey];
            }
            set
            {
                WriteSetting(_snippetsPathKey, value);
            }
        }

        private void WriteSetting(string key, string value)
        {
            _configuration = WebConfigurationManager.OpenWebConfiguration("~");
            _configuration.AppSettings.Settings.Remove(key);
            _configuration.AppSettings.Settings.Add(key, value);
            _configuration.Save();
        }
    }
}
