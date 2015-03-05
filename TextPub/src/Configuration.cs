using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Linq;
using TextPub.Models;

namespace TextPub
{
    public sealed class Configuration
    {
        private const string _basePathKey = "TextPub_BasePath";
        private const string _postsPathKey = "TextPub_PostsPath";
        private const string _pagesPathKey = "TextPub_PagesPath";
        private const string _snippetsPathKey = "TextPub_SnippetsPath";

        private System.Configuration.Configuration _configuration;

        internal Configuration()
        {
            if (HttpContext.Current != null) 
            {
                _configuration = WebConfigurationManager.OpenWebConfiguration(HostingEnvironment.ApplicationVirtualPath);
            }
            else 
            {
                // for unit testing
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                fileMap.ExeConfigFilename = "../../web.config";
                _configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            }

            // Set some defaults
            if (BasePath == null)
            { 
                BasePath = "~/App_Data";
            }

            if (PostsPath == null)
            { 
                PostsPath = "posts";
            }

            if (PostsPath == null)
            { 
                PagesPath = "pages";
            }

            if (SnippetsPath == null)
            { 
                SnippetsPath = "snippets";
            }
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
                return _configuration.GetAppSettingOrDefault(_postsPathKey);
            }
            set
            {
                _configuration.SaveAppSetting(_postsPathKey, value);
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
                return _configuration.GetAppSettingOrDefault(_pagesPathKey);
            }
            set
            {
                _configuration.SaveAppSetting(_pagesPathKey, value);
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
                return _configuration.GetAppSettingOrDefault(_snippetsPathKey);
            }
            set
            {
                _configuration.SaveAppSetting(_snippetsPathKey, value);
            }
        }

        public string BasePath
        {
            get
            {
                return _configuration.GetAppSettingOrDefault(_basePathKey);
            }
            set
            {
                _configuration.SaveAppSetting(_basePathKey, value);
            }
        }

        public Func<IPage, IPage> PageDecoratorProvider { get; set; }
        public Func<IPost, IPost> PostDecoratorProvider { get; set; }
        public Func<ISnippet, ISnippet> SnippetDecoratorProvider { get; set; }
    }
}
