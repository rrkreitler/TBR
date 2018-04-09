using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBrowser.Models;

namespace TweetBrowser.Services
{
    public interface IDataImport
    {
        IEnumerable<Tweet> GetItemsFromUrl(string url, DateTime startDate, DateTime endDate);
    }
}
