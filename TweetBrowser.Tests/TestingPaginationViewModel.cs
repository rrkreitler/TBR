using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Configuration;
using Moq;
using NUnit.Framework;
using TweetBrowser.Models;
using TweetBrowser.Services;
using Microsoft.Extensions.Configuration;

namespace TweetBrowser.Tests
{   
    [TestFixture]
    public class TestingPaginationViewModel
    {
        private Microsoft.Extensions.Configuration.IConfiguration _mockConfig;
        
        [SetUp]
        public void BeforeEachTest()
        {
            // Arrange mocked resources for all tests
            _mockConfig = new Mock<Microsoft.Extensions.Configuration.IConfiguration>().Object;
        }

        private ITweetBrowserData MockDbContext(int itemCount)
        {
            var mockDbContext = new Mock<ITweetBrowserData>();
            var tweets = TestItems(itemCount);
            mockDbContext.Setup(c => c.AllItems).Returns(tweets);
            return mockDbContext.Object;
        }

        // Testing the ShowNext and ShowPrevious methods
        [Test]
        public void ShowNextAdvancesStartIndexOnePageSize()
        {
            // Arrange
            PaginationViewModel sut = new PaginationViewModel(MockDbContext(35), _mockConfig );
            var testPageSize = 25;
            sut.PageSize = testPageSize;
            var startIndex = sut.StartIndex;
            
            // Act
            sut.ShowNext();

            Assert.That(sut.StartIndex.Equals(startIndex + testPageSize));
        }

        [Test]
        public void ShowNextDoesNotAdvanceStarIndexOnLastPage()
        {
            // Arrange
            PaginationViewModel sut = new PaginationViewModel(MockDbContext(35), _mockConfig);
            var testPageSize = 25;
            sut.PageSize = testPageSize;
            // Advance to last page
            sut.ShowNext();
            var startIndex = sut.StartIndex;

            // Act
            sut.ShowNext();

            Assert.That(sut.StartIndex.Equals(startIndex));
        }

       [Test]
        public void ShowPreviousSubtractsOnePageSizeFromStartIndex()
        {
            // Arrange
            PaginationViewModel sut = new PaginationViewModel(MockDbContext(35), _mockConfig);
            var testPageSize = 25;
            sut.PageSize = testPageSize;
            // Advance to last page
            sut.ShowNext();
            var startIndex = sut.StartIndex;

            // Act
            sut.ShowPrevious();

            Assert.That(sut.StartIndex.Equals(startIndex - testPageSize));
        }

        [Test]
        public void ShowPreviousDoesNotSubtractOnePageSizeOnFirstPage()
        {
            // Arrange
            PaginationViewModel sut = new PaginationViewModel(MockDbContext(35), _mockConfig);
            var testPageSize = 25;
            sut.PageSize = testPageSize;
            var startIndex = sut.StartIndex;

            // Act
            sut.ShowPrevious();

            Assert.That(sut.StartIndex.Equals(startIndex));
        }

        // Testing the ViewableItemsList
        [Test]
        public void NumberOfDefaultViewableItemsIsPageSize()
        {
            // Arrange
            PaginationViewModel sut = new PaginationViewModel(MockDbContext(35), _mockConfig);
            // Note: the number of items in the MockDbContext must be >= pagesize
            // for this test to pass.
            int pageSize = 25;
            sut.PageSize = pageSize;

            // Act
            sut.GetQueryItems();

            Assert.That(sut.ViewableItems.Count.Equals(pageSize));
        }

        [Test]
        public void NumberOfViewableItemsLessThanPageSizeWithSmallDb()
        {
            // Arrange
            PaginationViewModel sut = new PaginationViewModel(MockDbContext(15), _mockConfig);
            // Note: the number of items in the MockDbContext must be < pagesize
            // for this test to pass.
            int pageSize = 25;
            sut.PageSize = pageSize;

            // Act
            sut.GetQueryItems();

            Assert.That(sut.ViewableItems.Count.Equals(15));
        }


        [Test]
        public void ShowAllAddsAllItemsToViewableList()
        {
            // Arrange
            PaginationViewModel sut = new PaginationViewModel(MockDbContext(35), _mockConfig);
            sut.ShowAll = true;
            
            // Act
            sut.GetQueryItems();

            Assert.That(sut.ViewableItems.Count.Equals(35));
        }

        // Test Search impact on ViewableList
        [Test]
        public void SearchReturnsEmptyListIfNoItemsFound()
        {
            // Arrange
            PaginationViewModel sut = new PaginationViewModel(MockDbContext(10), _mockConfig);
            sut.SearchFilter = "Darth Vader";
            
            // Act
            sut.GetQueryItems();

            Assert.That(sut.ViewableItems.Count.Equals(0));
        }

        [Test]
        public void SearchFindsDistinctItems()
        {
            // Arrange
            PaginationViewModel sut = new PaginationViewModel(MockDbContext(35), _mockConfig);
            sut.SearchFilter = "1000";

            // Act
            sut.GetQueryItems();

            Assert.That(sut.ViewableItems.Count.Equals(1));
        }

        [Test]
        public void SearchFindsMultipleDistinctItems()
        {
            // Arrange
            PaginationViewModel sut = new PaginationViewModel(MockDbContext(10), _mockConfig);
            sut.SearchFilter = "1001";
            // The MockDbContext must have at least 3 items in it for this test to pass.

            // Act
            sut.GetQueryItems();

            Assert.That(sut.ViewableItems.Count.Equals(2));
        }

        // Test sorting options
        [Test]
        public void SortsByIdAscendingByDefault()
        {
            // Arrange
            PaginationViewModel sut = new PaginationViewModel(MockDbContext(10), _mockConfig);
            
            // Act
            sut.GetQueryItems();

            Assert.Multiple(() =>
            {
                Assert.That(sut.ViewableItems[0].Id.Equals("1000"));
                Assert.That(sut.ViewableItems[9].Id.Equals("1009"));
            });
        }

        [Test]
        public void SortsByIdDescending()
        {
            // Arrange
            PaginationViewModel sut = new PaginationViewModel(MockDbContext(10), _mockConfig);
            sut.SortOrder = "IdDescend";

            // Act
            sut.GetQueryItems();

            Assert.Multiple(() =>
            {
                Assert.That(sut.ViewableItems[0].Id.Equals("1009"));
                Assert.That(sut.ViewableItems[9].Id.Equals("1000"));
            });
        }

        [Test]
        public void SortsByDateAscending()
        {
            // Arrange
            PaginationViewModel sut = new PaginationViewModel(MockDbContext(10), _mockConfig);
            sut.SortOrder = "Date";

            // Act
            sut.GetQueryItems();

            Assert.Multiple(() =>
            {
                Assert.That(sut.ViewableItems[0].Id.Equals("1000"));
                Assert.That(sut.ViewableItems[9].Id.Equals("1009"));
            });
        }

        [Test]
        public void SortsByDateDescending()
        {
            // Arrange
            PaginationViewModel sut = new PaginationViewModel(MockDbContext(10), _mockConfig);
            sut.SortOrder = "DateDescend";

            // Act
            sut.GetQueryItems();

            Assert.Multiple(() =>
            {
                Assert.That(sut.ViewableItems[0].Id.Equals("1009"));
                Assert.That(sut.ViewableItems[9].Id.Equals("1000"));
            });
        }

        // Generate test items for the mock dbContext
        private List<Tweet> TestItems(int itemCount)
        {
            List<Tweet> testItems = new List<Tweet>();
            DateTime stamp = DateTime.Now;
            for(int i = 1000; i < 1000 + itemCount; i++)
            {
                testItems.Add( new Tweet() {Id = i.ToString(),
                                            Stamp = stamp.ToString(),
                                            Text = "Test message " + i.ToString()});
                stamp += TimeSpan.FromMinutes(2);
            };
            if (itemCount > 2)
            {
                testItems[2].Text = "Test message 1001";
            }

            return testItems;
        }
    }
}
