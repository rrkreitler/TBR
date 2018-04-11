using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TweetBrowser.Services;
using Microsoft.Extensions.Logging;


namespace TweetBrowser.Tests
{
    [TestFixture]
    public class TestingTweetDataClient
    {
        [Test]
        public void ThrowsExceptionOnInvalidUri()
        {
            // Arrange
            var mock = new Mock<ILogger<TweetDataClient>>();
            ILogger<TweetDataClient> logger = mock.Object;
            var sut = new TweetDataClient(logger);

            Assert.That(()=>sut.GetItemsFromUrl("",DateTime.Now, DateTime.Now),Throws.Exception);
        }

    }
}
