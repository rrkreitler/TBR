using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using TweetBrowser.Models;
using TweetBrowser.Services;

namespace TweetBrowser.Pages.Import
{
    public class IndexModel : PageModel
    {
        private readonly ITweetBrowserData _dbContext;
        private readonly IDataImport _remoteDataSrc;
        private readonly IConfiguration _configuration;

        public IndexModel(ITweetBrowserData dbContext, IDataImport remoteDataSrc, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _remoteDataSrc = remoteDataSrc;
            _configuration = configuration;
        }

        public IList<Tweet> Tweets { get; set; }
        public string Status { get; set; }

        public bool ShowImport { get; set; } = false;
        [BindProperty]
        [Display(Name = "Select Start Date")]
        [DataType(DataType.Date, ErrorMessage="This must be a valid date")]
        public DateTime StartDate { get; set; }
        [BindProperty]
        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        public DateTime StartTime { get; set; }
        [BindProperty]
        [Display(Name = "Select End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        [BindProperty]
        [Display(Name = "End Time")]
        [DataType(DataType.Time)]
        public DateTime EndTime { get; set; }


        public void OnGet()
        {
            ShowImport = true;
            Tweets = _dbContext.AllItems;
            StartDate = DateTime.Now;
            EndDate = DateTime.Now;
            StartTime = Convert.ToDateTime("00:00:00 AM");
            EndTime = Convert.ToDateTime("00:00:00 AM");
            Status = string.Empty;
        }

        [ValidateAntiForgeryToken]
        public IActionResult OnPostInspectAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            IEnumerable<Tweet> tweets = RetrieveDataFromRemoteSite();

            if (tweets == null || !tweets.Any())
            {
                Status = "Items found: 0";
            }
            else
            {
                Status = "Items found: " + tweets.Count();
            }

            return Page();
        }

        [ValidateAntiForgeryToken]
        public IActionResult OnPostImportAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            IEnumerable<Tweet> tweets = null;
            try
            {
                tweets = RetrieveDataFromRemoteSite();
            }
            catch (Exception e)
            {
                string msg = e.Message;
                return RedirectToPage("/Error", e.Message);
            }

            if (tweets == null || !tweets.Any())
            {
                Status = "Items imported: 0 - No items were found.";
            }
            else
            {
                // Add new items to local data store
                int count = _dbContext.AllItems.Count;
                int duplicatesFound = 0;

                foreach (var tweet in tweets)
                {
                    try
                    {
                        if (_dbContext.Add(tweet) == null)
                        {
                            duplicatesFound++;
                        }
                    }
                    catch
                    {
                        // Errors here will be the result of attempting to add duplicate Id's.
                        // This prevents duplicate objects in the data store. 
                        duplicatesFound++;
                    }
                }

                Status = "Items imported: " + (_dbContext.AllItems.Count - count);
                if (duplicatesFound == 0)
                {
                    Status +=  " - No duplicates were found.";
                }
                else
                {
                    Status += " - Duplicates not imported: " + duplicatesFound;
                }
            }

            return Page();
        }

        private IEnumerable<Tweet> RetrieveDataFromRemoteSite()
        {
            DateTime fullStartDate = Convert.ToDateTime(StartDate.ToShortDateString() + " " + StartTime.ToShortTimeString());
            DateTime fullEndDate = Convert.ToDateTime(EndDate.ToShortDateString() + " " + EndTime.ToShortTimeString());

            // Retrieve items from remote archive
            //return _remoteDataSrc.GetItemsFromUrl(_configuration["RemoteDataUri"], fullStartDate, fullEndDate);
            return _remoteDataSrc.GetItemsFromUrl("RemoteDataUri", fullStartDate, fullEndDate);
        }
    }
}