using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;

namespace TextPub
{
    public static class StringExtensions
    {
        /// <summary>
        /// Creates a "URL friendly" string by converting/stripping special characters. 
        /// An example: "This is a string which contains åäö" will be converted to "this-is-a-string-which-contains-aao".
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string UrlFriendly(this String input)
        {
            // Solution by Jon Skeet
            // http://stackoverflow.com/a/810359/321233
            string normalized = input.Normalize(NormalizationForm.FormKD);
            Encoding removal = Encoding.GetEncoding(Encoding.ASCII.CodePage, new EncoderReplacementFallback(string.Empty), new DecoderReplacementFallback(string.Empty));
            byte[] bytes = removal.GetBytes(normalized);

            // Additions by Jonathan Hedrén:
            // Replace all whitespaces with -.
            var result = Regex.Replace(Encoding.ASCII.GetString(bytes), @"[^A-Za-z0-9_\.~]+", "-");
            // Remove any "double" dashes before returning the url friendly string.
            return Regex.Replace(result, @"\-{2,}", "-").Trim('-').ToLower();
        }
    }
}