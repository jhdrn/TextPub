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
    internal class ConfigurationManager
    {
        private string _configPath = HostingEnvironment.MapPath("~/TextPub.config");
        private const string _selectorFormat = "/TextPub/{0}";

        internal void Put(string key, object value)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_configPath);

            XmlNode node = doc.SelectSingleNode(string.Format(_selectorFormat, key));
            node.InnerText = (string)value;

            XmlTextWriter writer = new XmlTextWriter(_configPath, null);
            writer.Formatting = Formatting.Indented;
            doc.WriteTo(writer);
            writer.Flush();
            writer.Close();
        }

        internal string Get(string key)
        {
            if (!File.Exists(_configPath))
            {
                CreateConfigFile();
            }
            var doc = new XmlDocument();
            doc.Load(_configPath);
            XmlNode node = doc.SelectSingleNode(string.Format(_selectorFormat, key));
            return node.InnerText;

        }

        private void CreateConfigFile()
        {
            XDocument doc = new XDocument(
                new XElement("TextPub",
                    new XElement(TextPub.DropBoxConsumerToken, ""),
                    new XElement(TextPub.DropBoxConsumerSecret, ""),
                    new XElement(TextPub.DropBoxUserToken, ""),
                    new XElement(TextPub.DropBoxUserSecret, ""),
                    new XElement(TextPub.DropBoxSyncInterval, ""),
                    new XElement(TextPub.DropBoxDeltaCursor, "")
                    
                )
            );
            doc.Save(_configPath);
        }

    }
}
