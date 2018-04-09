using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TweetBrowser.Models;


namespace TweetBrowser.Services
{
    public class TweetDataClient : IDataImport
    {
        private readonly HttpClient client = new HttpClient();
        private TimeSpan _maxTimeSpan;

        private async Task<ICollection<Tweet>> GetTweetsAsync(string path)
        {
            ICollection<Tweet> tweets = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                tweets = await response.Content.ReadAsJsonAsync<ICollection<Tweet>>();
            }
            return tweets;
        }

        private async Task<IEnumerable<Tweet>> RunAsync(string uriString, DateTime startDate, DateTime maxDate)
        {
            DateTime endDate = maxDate;
            _maxTimeSpan = endDate.Subtract(startDate);
            
            if (client.BaseAddress == null)
            {
                // client.BaseAddress = new Uri("https://badapi.iqvia.io/");
                client.BaseAddress = new Uri(uriString);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            }

            try
            {
                List<Tweet> allTweets = new List<Tweet>();
                bool doneQuerying = false;

                while (!doneQuerying)
                {
                    UriBuilder uriBuilder = new UriBuilder(uriString);
                    uriBuilder.Port = -1;
                    uriBuilder.Query = "startDate=" + startDate.ToString() + "&endDate=" + endDate.ToString();
                    Uri uri = new Uri(uriBuilder.ToString());

                    // Get the tweets
                    IEnumerable<Tweet> tweets = null;
                    tweets = await GetTweetsAsync(uri.PathAndQuery);

                    // Check to see if the Tweet API's 100 record limit was reached.
                    if (!MaxRecordsExceeded(tweets))
                    {
                        allTweets.AddRange(tweets);
                        doneQuerying = endDate == maxDate;
                        if (!doneQuerying)
                        {
                                startDate = IncrementDate(startDate, maxDate);
                                endDate = IncrementDate(endDate, maxDate);
                        }
                    }
                    else
                    {
                        // Adjust endDate based on new _maxTimespan
                        endDate = IncrementDate(startDate, maxDate);
                        Debug.WriteLine("===============================================================");
                        Debug.WriteLine("StartDate: " + startDate + "   EndDate: " + endDate + "   Interval: " + _maxTimeSpan.TotalDays);
                        Debug.WriteLine("===============================================================");
                    }
                }
                
                return allTweets;

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return null;
        }

        string span = "Default";
        private bool MaxRecordsExceeded(IEnumerable<Tweet> tweets)
        {
            // If 100 records were returned it is likely the Tweet API's 100 record
            // limitation was reached. This reduces the timespan of the query
            // to reduce the number of records returned to be less than 100. 
            // It then submits mulitple queries until the entire time span of the orignal
            // query has been processed.
            Debug.WriteLine("===============================================================");
            Debug.WriteLine("Count: " + tweets.Count() + "   Span:" + span);
            Debug.WriteLine("===============================================================");
            if (tweets.Count() >= 100)
            {
                if (_maxTimeSpan > TimeSpan.FromDays(3))
                {
                    _maxTimeSpan = TimeSpan.FromDays(3);
                    span = "3 days";
                }
                else if (_maxTimeSpan > TimeSpan.FromDays(2))
                {
                    _maxTimeSpan = TimeSpan.FromDays(2);
                    span = "2 days";
                }
                else if (_maxTimeSpan > TimeSpan.FromDays(1))
                {
                    _maxTimeSpan = TimeSpan.FromDays(1);
                    span = "1 day";
                }
                else if (_maxTimeSpan > TimeSpan.FromHours(12))
                {
                    _maxTimeSpan = TimeSpan.FromHours(12);
                    span = "12 hours";
                }
                else if (_maxTimeSpan > TimeSpan.FromHours(1))
                {
                    _maxTimeSpan = TimeSpan.FromHours(1);
                    span = "1 hour";
                }
                else if (_maxTimeSpan > TimeSpan.FromMinutes(30))
                {
                    _maxTimeSpan = TimeSpan.FromMinutes(30);
                    span = "30 mins";
                }
                else
                {
                    // Timespan could be throttled even further but at this point
                    // performance will be so bad another solution should be considered.
                    throw new TweetWebApiException("Unrealiable host data. Records may be missing.");
                }

                return true;
            }

            return false;
        }

        private DateTime IncrementDate(DateTime date, DateTime maxDate)
        {
            if (date + _maxTimeSpan > maxDate)
            {
                return maxDate;
            }

            return date + _maxTimeSpan;
        }

        public IEnumerable<Tweet> GetItemsFromUrl(string url, DateTime startDate, DateTime endDate)
        {
            IEnumerable<Tweet> tweets = RunAsync(url, startDate, endDate).GetAwaiter().GetResult();
            return tweets;
        }
    }

    public static class HttpContentExtensions
    {
        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
        {
            string json = await content.ReadAsStringAsync();
            T value = JsonConvert.DeserializeObject<T>(json);
            return value;
        }
    }

    public class TweetWebApiException : Exception
    {
        public TweetWebApiException(string message) : base(message)
        {

        }
    }
}
