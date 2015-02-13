using System;
using System.Collections.Generic;
using System.Linq;
using MarkdownDeep;

namespace TextPub
{
    /// <summary>
    /// Wrapper for MarkdownDeep
    /// </summary>
    internal static class MarkdownHelper
    {
        public static string Transform(string input)
        {
            Markdown markdown = new Markdown();

            markdown.ExtraMode = true;
            markdown.SafeMode = true;

            return markdown.Transform(input);
        }
    }
}