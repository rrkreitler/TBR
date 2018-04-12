using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Isam.Esent.Interop;
using TweetBrowser.Models;
using TweetBrowser.Services;

namespace TweetBrowser.Pages.Import
{
    // This is the Import page. This page allows users to 
    // input dates and send queries to a remote site
    // (URL in the appsettings.json file).
    // Users have two options: INSPECT, allows them
    // to see how many records will be returned with their 
    // query, and IMPORT which performs the query and copies
    // any records returned into the local data store.
    // Incoming records are checked and duplicates are 
    // ignored.
    public class IndexModel : PageModel
    {
        private readonly ITweetBrowserData _dbContext;
        private readonly IDataImport _remoteDataSrc;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public IndexModel(ITweetBrowserData dbContext, IDataImport remoteDataSrc, IConfiguration configuration, ILogger<IndexModel> logger)
        {
            _dbContext = dbContext;
            _remoteDataSrc = remoteDataSrc;
            _configuration = configuration;
            _logger = logger;
        }

        public IList<Tweet> Tweets { get; set; }
        public string Status { get; set; }
        public bool ShowImport { get; set; } = false;

        [BindProperty]
        [Display(Name = "Select Start Date")]
        [DataType(DataType.Date)]
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
        
        // Default Action.
        public void OnGet()
        {
            ShowImport = true;
            Tweets = _dbContext.AllItems;
            Status = string.Empty;
            // Set default time values for the import screen.
            StartDate = DateTime.Now;
            EndDate = DateTime.Now;
            StartTime = Convert.ToDateTime("00:00:00 AM");
            EndTime = Convert.ToDateTime("00:00:00 AM");
        }

        // Action for the INSPECT button.
        [ValidateAntiForgeryToken]
        public IActionResult OnPostInspectAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Query the remote site.
            IEnumerable<Tweet> tweets = null;
            try
            {
                tweets = RetrieveDataFromRemoteSite();
            }
            catch (Exception e)
            {
                return RedirectToPage("/Error", "", new { message = e.Message });
            }
            // Update status on page.
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

        // Action for the IMPORT button.
        [ValidateAntiForgeryToken]
        public IActionResult OnPostImportAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Query the remote site.
            IEnumerable<Tweet> tweets = null;
            try
            {
                tweets = RetrieveDataFromRemoteSite();
            }
            catch (Exception e)
            {
                return RedirectToPage("/Error","",new {message = e.Message});
            }

            // Update the status on the page.
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
                    // Errors here will be the result of attempting to add duplicate Id's.
                    try
                    {
                        if (_dbContext.Add(tweet) == null)
                        {
                            duplicatesFound++;
                        }
                    }
                    catch
                    {
                        // Normally this would catch concurrency errors and duplicate key
                        // errors from the ORM/database being used on the backend. This demo uses
                        // a simple in-memory collection as the local data store that does not
                        // throw an exception when a duplicate is found, instead it returns a null
                        // object (this improves performance). This counts the nulls returned to 
                        // track dupes.
                        duplicatesFound++;
                    }
                }
                // Update status on page with import summary.
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
            try
            {
                DateTime fullStartDate =
                    Convert.ToDateTime(StartDate.ToShortDateString() + " " + StartTime.ToShortTimeString());
                DateTime fullEndDate =
                    Convert.ToDateTime(EndDate.ToShortDateString() + " " + EndTime.ToShortTimeString());

                // Retrieve items from the remote archive with the URL in the appsettings.json file.
                return _remoteDataSrc.GetItemsFromUrl(_configuration["RemoteDataUri"], fullStartDate, fullEndDate);

            }
            catch (TweetWebApiException te)
            {
                // This exception is thrown because too many records are being returned
                // in each query after the remote data client has throttled back as far as it can
                // go. Records may be being lost.
                _logger.LogError(te, "Query request to remote site: {url} possibly missing data", _configuration["RemoteDataUri"]);
                throw new Exception(te.Message + " It is recommended you try accessing the data using a different means.");
            }
            catch (Exception ex)
            {
                // Dates are validated before being submitted so if an error occurs at this
                // point it will be a problem with the url in the appsettings.json file
                // or with general connectivity to the remote site itself.
                _logger.LogWarning(ex, "Query request to remote site: {url} connection problem", _configuration["RemoteDataUri"]);
                throw new Exception(
                    "There is a problem accessing the remote site. Make sure that the URL is correct in the appsettings file and that the remote site is online and then try again.",ex);
            }
            
        }
    }
}