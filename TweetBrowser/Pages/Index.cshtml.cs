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
    public class IndexModel : PageModel
    {
        private readonly ITweetBrowserData _dbContext;

        public IndexModel(ITweetBrowserData dbContext )
        {
            _dbContext = dbContext;
            PaginationViewModel = new PaginationViewModel(_dbContext,25);
        }

        public bool LocalDataExists => _dbContext.AllItems.Any();
        public List<Tweet> Tweets { get; set; }
        public PaginationViewModel PaginationViewModel { get; private set; }
        
        // Default view
        public void OnGetAsync(string sortOrder,string searchString, bool searchVisible)
        {
            // Shows and hides the search controls
            ToggleSearchMenu();
            
            PaginationViewModel.ShowSearch = searchVisible;
            PaginationViewModel.SearchFilter = searchString;
            PaginationViewModel.SortOrder = sortOrder;

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
