using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBrowser.Models;

namespace TweetBrowser.Services
{
    public interface ITweetBrowserData
    {
        // For a real deployment this interface would represent the 
        // persistence layer. I.E. DbContext.
        List<Tweet> AllItems { get; }
        Tweet Add(Tweet newItem);
    }
}
