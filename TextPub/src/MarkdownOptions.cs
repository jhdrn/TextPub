using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextPub
{
    public sealed class MarkdownOptions
    {
        public static MarkdownOptions Create()
        {
            return new MarkdownOptions
            {
                ExtraMode = true
            };
        }

        /// <summary>
        /// Set to true to only allow whitelisted safe html tags
        /// </summary>
        public bool SafeMode { get; set; }

        /// <summary>
        /// Set to true to enable ExtraMode, which enables the same set of features as implemented 
        /// by PHP Markdown Extra.
        /// </summary>
        public bool ExtraMode { get; set; }

        /// <summary>
        /// When set, all html block level elements automatically support markdown syntax within 
        /// them. (Similar to Pandoc's handling of markdown in html)
        /// </summary>
        public bool MarkdownInHtml { get; set; }

        /// <summary>
        /// When set, all headings will have an auto generated ID attribute based on the heading 
        /// text (uses the same algorithm as Pandoc)
        /// </summary>
        public bool AutoHeadingIDs { get; set; }

        /// <summary>
        /// When set, all non-qualified urls (links and images) will
        /// be qualified using this location as the base.
        /// Useful when rendering RSS feeds that require fully qualified urls.
        /// </summary>
        public string UrlBaseLocation { get; set; }

        /// <summary>
        /// When set, all non-qualified urls (links and images) begining with a slash
        /// will qualified by prefixing with this string.
        /// Useful when rendering RSS feeds that require fully qualified urls. 
        /// </summary>
        public string UrlRootLocation { get; set; }

        /// <summary>
        /// When true, all fully qualified urls will be give `target="_blank"' attribute causing 
        /// them to appear in a separate browser window/tab (ie: relative links open in same 
        /// window, qualified links open externally)
        /// </summary>
        public bool NewWindowForExternalLinks { get; set; }

        /// <summary>
        /// When true, all urls (qualified or not) will get target="_blank" attribute (useful for 
        /// preview mode on posts)
        /// </summary>
        public bool NewWindowForLocalLinks { get; set; }

        /// <summary>
        /// When set, will try to determine the width/height for local images by searching for an 
        /// appropriately named file relative to the specified location Local file system location 
        /// of the document root. Used to locate image files that start with slash.
        ///
        /// Typical value: c:\inetpub\www\wwwroot
        /// </summary>
        public string DocumentRoot { get; set; }

        /// <summary>
        /// Local file system location of the current document. Used to locate relative path 
        /// images for image size.
        /// 
        /// Typical value: c:\inetpub\www\wwwroot\subfolder
        /// </summary>
        public string DocumentLocation { get; set; }

        /// <summary>
        /// Limit the width of images (0 for no limit). Only used when image sizes are read from 
        /// DocumentRoot or DocumentLocation
        /// </summary>
        public int MaxImageWidth { get; set; }

        /// <summary>
        /// When set, all links get rel="nofollow" attribute
        /// </summary>
        public bool NofollowLinks { get; set; }
    }
}
