using System;
using System.Linq;
using TextPub;
using Xunit;

namespace TextPub.Tests
{
    public class PostTests
    {
        public PostTests()
        {
            Application.Configuration.BasePath = "../../App_Data";
        }

        [Fact]
        public void TestPostWithTitle()
        {
            Assert.Equal(Application.Posts["post"].Title, "Post title");
            Assert.Equal(Application.Posts["post-without-title"].Body, "<p>The post body...</p>\n");
            Assert.NotNull(Application.Posts["post-with-publish-date"].PublishDate);
        }

        [Fact]
        public void TestPostWithoutTitle()
        {
            Assert.Equal(Application.Posts["post-without-title"].Title, "post-without-title");
            Assert.Equal(Application.Posts["post-without-title"].Body, "<p>The post body...</p>\n");
            Assert.NotNull(Application.Posts["post-with-publish-date"].PublishDate);
        }

        [Fact]
        public void TestPostWithPublishDate()
        {
            Assert.Equal(Application.Posts["post-with-publish-date"].Title, "Post title");
            Assert.Equal(Application.Posts["post-with-publish-date"].PublishDate, DateTime.Parse("2015-01-01"));
            Assert.Equal(Application.Posts["post-without-title"].Body, "<p>The post body...</p>\n");
        }

        [Fact]
        public void TestPostTitleWithEmptyTitle()
        {
            Assert.Equal(Application.Posts["post-with-empty-title"].Title, "post-with-empty-title");
            Assert.Equal(Application.Posts["post-without-title"].Body, "<p>The post body...</p>\n");
            Assert.NotNull(Application.Posts["post-with-publish-date"].PublishDate);
        }

        [Fact]
        public void TestPostTitleWithDateTitle()
        {
            Assert.Equal(Application.Posts["post-with-date-title"].Title, "post-with-date-title");
            Assert.Equal(Application.Posts["post-with-date-title"].PublishDate, DateTime.Parse("2015-01-01"));
            Assert.Equal(Application.Posts["post-without-title"].Body, "<p>The post body...</p>\n");
        }
    }
}
