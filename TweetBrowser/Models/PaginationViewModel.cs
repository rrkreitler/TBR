using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TweetBrowser.Services;

namespace TweetBrowser.Models
{
    // This class provides the paginated view of the items in the local data store.
    // It keeps track of all the parameters needed to persist the view state
    // each time the page is updated.
    public class PaginationViewModel
    {
        private readonly ITweetBrowserData _dbContext;
        private List<Tweet> _viewableItems;
        private int _totalItems;


        public PaginationViewModel(ITweetBrowserData dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _viewableItems = new List<Tweet>();
            SearchFilter = String.Empty;
            SortOrder = "";
            _totalItems = _dbContext.AllItems.Count;
            StartIndex = 0;
            ShowAll = false;
            // Get default page from configuration in appsettings.json
            if (int.TryParse(configuration["PageSize"], out var pageSize))
            {
                PageSize = pageSize;
            }
            else
            {
                // If setting is not found set default to 25.
                PageSize = 25;
            }
        }

        public int PageSize { get; set; }
        public List<Tweet> ViewableItems => _viewableItems;
        public string SearchFilter { get; set; }
        public string SortOrder { get; set; }
        public bool ShowingFirstPage => StartIndex == 0;
        public bool ShowingLastPage => (_totalItems - 1) - StartIndex < PageSize;
        public bool ShowSearch { get; set; }
        public bool ShowAll { get; set; }
        public int StartIndex { get; set; }

        private int EndIndex
        {
            get
            {
                int endIndex = StartIndex + PageSize - 1;
                if (endIndex > _totalItems - 1)
                {
                    endIndex = _totalItems - 1;
                }
                return endIndex;
            }
        }

        public int[] Range
        {
            get
            {
                return new int[] {StartIndex + 1, EndIndex + 1, _totalItems};
            }
        }

        
        // Increment the index for the next page.
        public void ShowNext()
        {
            if (!ShowingLastPage)
            {
                StartIndex += PageSize;
            }
            GetQueryItems();
        }

        // Decrement the index for the previous page.
        public void ShowPrevious()
        {
            if (!ShowingFirstPage)
            {
                StartIndex -= PageSize;
                if (StartIndex < 0)
                {
                    StartIndex = 0;
                }
                GetQueryItems();
            }
        }


        // NOTE: The three methods below (GetQueryItems, SelectViewableItems, SortViewableList)
        // retrieve the data from the local data store, sort it based on the current settings,
        // and select the range of records to display.

        // Given the simple nature of the local datastore used for this demo, performance is 
        // not a concern. If this was a real-world situation where the back end was a remote database
        // and the sample data set was potentially much larger, a different approach would be needed
        // for these three methods.

        public void GetQueryItems()
        {
            _totalItems = 0;
            if (_dbContext.AllItems.Count != 0)
            {
                // Get filtered query results
                // Build query list
                IQueryable<Tweet> queryResult = _dbContext.AllItems.AsQueryable();
                if (!String.IsNullOrWhiteSpace(SearchFilter))
                {
                    queryResult = queryResult.Where(t => t.Id.Contains(SearchFilter)
                                                         || t.Stamp.Contains(SearchFilter)
                                                         || t.Text.Contains(SearchFilter)).Distinct();
                }

                _totalItems = queryResult.Count();
                
                // Sort items.
                queryResult= SortViewableList(queryResult);
                
                // Select items for the current view.
                SelectViewableItems(queryResult);
            }

        }

        private void SelectViewableItems(IQueryable<Tweet> queryResult)
        {
            if (ShowAll)
            {
                StartIndex = 0;
                PageSize = queryResult.Count();
                _viewableItems = queryResult.ToList();
                return;
            }

            // Select the items in the viewable range.
            _viewableItems = new List<Tweet>();

            for (int i = StartIndex; i <= EndIndex; i++)
            {
                _viewableItems.Add(queryResult.ElementAt(i));
            }
        }


        private IQueryable<Tweet> SortViewableList( IQueryable<Tweet> queryResult)
        {
            switch (SortOrder)
            {
                case "IdDescend":
                    return queryResult.OrderByDescending(t => t.Id);
                case "Date":
                    return queryResult.OrderBy(t => t.Date);
                case "DateDescend":
                    return queryResult.OrderByDescending(t => t.Date);
                default:
                    return queryResult.OrderBy(t => t.Id);
            }
        }
    }
}
