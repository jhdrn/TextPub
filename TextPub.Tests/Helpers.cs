using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextPub.Tests
{
    static class Helpers
    {
        internal static void MockConfiguration()
        {
            var mock = new Mock<IConfiguration>()
                .SetupProperty(p => p.BasePath, "../../App_Data")
                .SetupProperty(p => p.PagesPath, "pages")
                .SetupProperty(p => p.PostsPath, "posts")
                .SetupProperty(p => p.SnippetsPath, "snippets");

            Application.Configuration = mock.Object;
        }
    }
}
