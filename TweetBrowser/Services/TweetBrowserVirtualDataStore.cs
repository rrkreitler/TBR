using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBrowser.Models;

namespace TweetBrowser.Services
{
    // This class provides a simple in-memory datastore for 
    // use in the demo application. The ITweetBrowserData interface
    // is used so this module can be replaced if need be and to
    // faciliate testing via IOC.
    public class TweetBrowserVirtualDataStore : ITweetBrowserData
    {
        // Main collection of item in the data store.
        // This is essentially the dbSet for tweets if EF were being used.
        public List<Tweet> AllItems { get; } = new List<Tweet>();

        public Tweet Add(Tweet newItem)
        {
            var dupeItem = AllItems.FirstOrDefault(i => i.Id == newItem.Id);
            if (dupeItem == null)
            {
                AllItems.Add(newItem);
                return newItem;
            }
            // For performance reasons this returns a null value if 
            // there is an attempt to add a duplicate item.
            // An exception could be thrown here to simulate a duplicate
            // key error that would normally come from an ORM.
            return null;
        }
    }
}
