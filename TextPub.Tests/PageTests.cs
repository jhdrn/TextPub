using Moq;
using System;
using System.Linq;
using TextPub;
using Xunit;

namespace TextPub.Tests
{
    public class PageTests
    {
        public PageTests()
        {
            Helpers.MockConfiguration();
        }

        [Fact]
        public void TestPageWithTitle()
        {
            var page = Application.Pages["page"];
            Assert.Equal(page.Title, "Page title");
            Assert.Equal(page.Body, "\n<p>The page body...</p>\n");
        }

        [Fact]
        public void TestPageWithoutTitle()
        {
            var page = Application.Pages["page-without-title"];
            Assert.Equal(page.Title, "page-without-title");
            Assert.Equal(page.Body, "<p>The page body...</p>\n");
        }

        [Fact]
        public void TestPageWithSortOrder()
        {
            var page = Application.Pages["page-with-sort-order"];
            Assert.Equal(page.Title, "Page title");
            Assert.Equal(page.SortOrder, 4);
            Assert.Equal(page.Body, "\n<p>The page body...</p>\n");
        }

        [Fact]
        public void TestPageWithEmptyTitle()
        {
            var page = Application.Pages["page-with-empty-title"];
            Assert.Equal(page.Title, "page-with-empty-title");
            Assert.Equal(page.Body, "\n<p>The page body...</p>\n");
        }

        [Fact]
        public void TestPageWithSortOrderNoTitle()
        {
            var page = Application.Pages["page-with-sort-order-no-title"];
            Assert.Equal(page.Title, "page-with-sort-order-no-title");
            Assert.Equal(page.SortOrder, 2);
            Assert.Equal(page.Body, "\n<p>The page body...</p>\n");
        }
    }
}
