using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MarkdownDeep;

namespace TextPub
{
    public static class MarkdownHelper
    {
        private static Markdown _markdown;
        static MarkdownHelper()
        {
            _markdown = new Markdown();

            _markdown.ExtraMode = true;
            _markdown.SafeMode = true;
        }


        public static string Transform(string input)
        {
            return _markdown.Transform(input);
        }
    }
}