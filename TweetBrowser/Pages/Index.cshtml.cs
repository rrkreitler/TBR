using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TweetBrowser.Models;
using TweetBrowser.Services;

namespace TweetBrowser.Pages
{
    // This is the the default page for the application. 
    // This is where users can view the items in the local data store.
    // A search option allows some basic filtering and the list
    // of items can be sorted by Id or by date.
    public class IndexModel : PageModel
    {
        private readonly ITweetBrowserData _dbContext;

        public IndexModel(ITweetBrowserData dbContext, IConfiguration configuration, PaginationViewModel pvm )
        {
            _dbContext = dbContext;
            PaginationViewModel = pvm;
        }

        public bool LocalDataExists => _dbContext.AllItems.Any();
        public List<Tweet> Tweets { get; set; }
        public PaginationViewModel PaginationViewModel { get; private set; }
        
        // Default view
        public void OnGetAsync(string sortOrder,string searchString, bool searchVisible, bool showAll)
        {
            // Shows and hides the search controls
            ToggleSearchMenu();
            
            PaginationViewModel.ShowSearch = searchVisible;
            PaginationViewModel.SearchFilter = searchString;
            PaginationViewModel.SortOrder = sortOrder;
            PaginationViewModel.ShowAll = showAll;

            if (LocalDataExists)
            {
                PaginationViewModel.GetQueryItems();
            }
        }

        // Handler for submission of the search form
        [ValidateAntiForgeryToken]
        public void OnPostAsync(bool searchVisible, string searchString)
        {
            PaginationViewModel.SearchFilter = searchString;
            PaginationViewModel.ShowSearch = searchVisible;
            PaginationViewModel.GetQueryItems();
        }

        // Handler for the Previous Page button
        public void OnGetShowPrevious(int pg,bool showSearch, string searchString, string sortOrder)
        {
            PaginationViewModel.StartIndex = pg;
            PaginationViewModel.ShowSearch = showSearch;
            PaginationViewModel.SearchFilter = searchString;
            PaginationViewModel.SortOrder = sortOrder;
            PaginationViewModel.ShowPrevious();
            ToggleSearchMenu();
        }
        // Handler for the Next Page button
        public void OnGetShowNext(int pg,bool showSearch, string searchString, string sortOrder)
        {
            PaginationViewModel.StartIndex = pg;
            PaginationViewModel.ShowSearch = showSearch;
            PaginationViewModel.SearchFilter = searchString;
            PaginationViewModel.SortOrder = sortOrder;
            PaginationViewModel.ShowNext();
            ToggleSearchMenu();
        }

        // Toggles the Search option off and on in the menu bar
        // based on whether or not there are records available to search.
        // The search button in the menu bar toggles the ShowSearch
        // property on the PaginationViewModel. This is a bool value
        // used to show/hide the SearchPartial partial view that 
        // renders the Search options on the main page.
        private void ToggleSearchMenu()
        {
            if (LocalDataExists)
            {
                ViewData["searchMenu"] = "visible";
            }
            else
            {
                ViewData["searchMenu"] = "hidden";
            }
        }

    }
}
