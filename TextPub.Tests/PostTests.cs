using Moq;
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
            Helpers.MockConfiguration();
        }

        [Fact]
        public void TestPostWithTitle()
        {
            var post = Application.Posts["post"];
            Assert.Equal("Post title", post.Title);
            Assert.Equal("\n<p>The post body...</p>\n", post.Body);
            Assert.NotNull(post.PublishDate);
        }

        [Fact]
        public void TestPostWithoutTitle()
        {
            var post = Application.Posts["post-without-title"];
            Assert.Equal("post-without-title", post.Title);
            Assert.Equal("<p>The post body...</p>\n", post.Body);
            Assert.NotNull(post.PublishDate);
        }

        [Fact]
        public void TestPostWithPublishDate()
        {
            var post = Application.Posts["post-with-publish-date"];
            Assert.Equal(post.Title, "Post title");
            Assert.Equal(post.PublishDate, DateTime.Parse("2015-01-01"));
            Assert.Equal(post.Body, "\n<p>The post body...</p>\n");
        }

        [Fact]
        public void TestPostWithEmptyTitle()
        {
            var post = Application.Posts["post-with-empty-title"];
            Assert.Equal(post.Title, "post-with-empty-title");
            Assert.Equal(post.Body, "\n<p>The post body...</p>\n");
            Assert.NotNull(post.PublishDate);
        }

        [Fact]
        public void TestPostWithDateTitle()
        {
            var post = Application.Posts["post-with-date-title"];
            Assert.Equal(post.Title, "post-with-date-title");
            Assert.Equal(post.PublishDate, DateTime.Parse("2015-01-01"));
            Assert.Equal(post.Body, "\n<p>The post body...</p>\n");
        }
    }
}
