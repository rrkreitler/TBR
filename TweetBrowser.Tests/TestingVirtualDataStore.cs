﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TweetBrowser.Models;
using TweetBrowser.Services;

namespace TweetBrowser.Tests
{
    [TestFixture]
    public class TestingVirtualDataStore
    {
        [Test]
        public void DefaultCollectionIsEmptyNotNull()
        {
            var sut = new TweetBrowserVirtualDataStore();

            Assert.That(sut.AllItems.Count.Equals(0));
        }

        [Test]
        public void AddMethodAddsItemToCollection()
        {
            var sut = new TweetBrowserVirtualDataStore();
            var testId = "09113456";

            Tweet tweet = new Tweet()
            {
                Id = testId ,
                Stamp = "2018/3/24 01:15 AM",
                Text = "Test message"
            };

            sut.Add(tweet);

            var item = sut.AllItems.FirstOrDefault(t => t.Id == testId);

            Assert.That(tweet.Equals(item));
        }

        [Test]
        public void AddMethodDoesNotAddDuplicatesItems()
        {
            var sut = new TweetBrowserVirtualDataStore();
            
            Tweet tweet = new Tweet()
            {
                Id = "09113456",
                Stamp = "2018/3/24 01:15 AM",
                Text = "Test message"
            };

            sut.Add(tweet);
            var count = sut.AllItems.Count;
            sut.Add(tweet);

            Assert.That(count.Equals(sut.AllItems.Count));
        }

        [Test]
        public void AddMethodReturnsItemAdded()
        {
            var sut = new TweetBrowserVirtualDataStore();
            var testId = "09113456";

            Tweet tweet = new Tweet()
            {
                Id = testId,
                Stamp = "2018/3/24 01:15 AM",
                Text = "Test message"
            };

            var item = sut.Add(tweet);

            Assert.That(tweet.Equals(item));
        }

        [Test]
        public void AddMethodReturnsNullOnAttemptToAddDuplicate()
        {
            var sut = new TweetBrowserVirtualDataStore();
            var testId = "09113456";

            Tweet tweet = new Tweet()
            {
                Id = testId,
                Stamp = "2018/3/24 01:15 AM",
                Text = "Test message"
            };

            sut.Add(tweet);
            var item = sut.Add(tweet);

            Assert.IsNull(item);
        }
    }
}
