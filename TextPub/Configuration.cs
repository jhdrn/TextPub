using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.Xml;
using System.Xml.Linq;

namespace TextPub
{
    /// <summary>
    /// A representation of ~/TextPub.config.
    /// </summary>
    public class Configuration
    {
        public const string DropBoxConsumerTokenKey = "DropBoxConsumerToken";
        public const string DropBoxConsumerSecretKey = "DropBoxConsumerSecret";
        public const string DropBoxUserTokenKey = "DropBoxUserToken";
        public const string DropBoxUserSecretKey = "DropBoxUserSecret";
        public const string DropBoxSyncIntervalKey = "DropBoxSyncInterval";
        internal const string DropBoxDeltaCursorKey = "DropBoxDeltaCursor";
        public const string ArticlesPathKey = "ArticlesPath";
        public const string PagesPathKey = "PagesPath";
        public const string SnippetsPathKey = "SnippetsPath";

        private string _configPath = HostingEnvironment.MapPath("~/TextPub.config");

        internal Configuration()
        {
            // Set some defaults
            ArticlesPath = "articles";
            PagesPath = "pages";
            SnippetsPath = "snippets";

            // Load config
            Load();
        }

        public string DropBoxConsumerToken { get; set; }
        public string DropBoxConsumerSecret { get; set; }
        public string DropBoxUserToken { get; set; }
        public string DropBoxUserSecret { get; set; }
        public int DropBoxSyncInterval { get; set; }
        internal string DropBoxDeltaCursor { get; set; }

        public string ArticlesPath { get; set; }
        public string PagesPath { get; set; }
        public string SnippetsPath { get; set; }

        public void Save()
        {
            WriteConfigFile();
        }

        private void Load()
        {
            if (File.Exists(_configPath))
            {
                ReadConfigFile();
            }
        }

        private void ReadConfigFile()
        {
            var doc = XDocument.Load(_configPath);

            DropBoxConsumerSecret = GetNodeValue(doc, DropBoxConsumerTokenKey);
            DropBoxConsumerToken = GetNodeValue(doc, DropBoxConsumerTokenKey);
            DropBoxUserSecret = GetNodeValue(doc, DropBoxUserSecretKey);
            DropBoxUserToken = GetNodeValue(doc, DropBoxUserTokenKey);

            string syncIntervalSetting = GetNodeValue(doc, DropBoxSyncIntervalKey);
            int syncInterval;
            if (!int.TryParse(syncIntervalSetting, out syncInterval))
            {
                // Sync every hour per default
                syncInterval = 60;
            }
            DropBoxSyncInterval = syncInterval;
            DropBoxDeltaCursor = GetNodeValue(doc, DropBoxDeltaCursorKey);

            ArticlesPath = GetNodeValue(doc, ArticlesPathKey);
            PagesPath = GetNodeValue(doc, PagesPathKey);
            SnippetsPath = GetNodeValue(doc, SnippetsPathKey);

        }

        private static string GetNodeValue(XDocument doc, string key)
        {
            return doc.Root.Descendants(key).Single().Value;
        }
        
        private XDocument WriteConfigFile()
        {
            XDocument doc = new XDocument(
                new XElement("TextPub",
                    new XElement(DropBoxConsumerTokenKey, DropBoxConsumerToken),
                    new XElement(DropBoxConsumerSecretKey, DropBoxConsumerSecret),
                    new XElement(DropBoxUserTokenKey, DropBoxUserToken),
                    new XElement(DropBoxUserSecretKey, DropBoxUserSecret),
                    new XElement(DropBoxSyncIntervalKey, Convert.ToString(DropBoxSyncInterval)),
                    new XElement(DropBoxDeltaCursorKey, DropBoxDeltaCursor),
                    new XElement(ArticlesPathKey, ArticlesPath),
                    new XElement(PagesPathKey, PagesPath),
                    new XElement(SnippetsPathKey, SnippetsPath)
                )
            );
            
            doc.Save(_configPath);
            return doc;
        }

    }
}
