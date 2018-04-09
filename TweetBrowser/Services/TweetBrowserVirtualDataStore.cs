using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBrowser.Models;

namespace TweetBrowser.Services
{
    public class TweetBrowserVirtualDataStore : ITweetBrowserData
    {
        public List<Tweet> AllItems { get; } = new List<Tweet>();

        public TweetBrowserVirtualDataStore()
        {
            //AllItems = TestData();
        }

        public Tweet Add(Tweet newItem)
        {
            var dupeItem = AllItems.FirstOrDefault(i => i.Id == newItem.Id);
            if (dupeItem == null)
            {
                AllItems.Add(newItem);
                return newItem;
            }
            //throw new Exception("Cannot add item. Id already exists in data store.");
            return null;
        }

        private List<Tweet> TestData()
        {
            List<Tweet> testData = new List<Tweet>();
            Random rnd = new Random();
            int id = rnd.Next(100, 200);
            for (int y = 2016; y < 2019; y++)
            {
                for (int m = 1; m < 13; m++)
                {
                    int day = rnd.Next(1, 28);
                    string txt;
                    try
                    {
                        txt = text.Substring(rnd.Next(0, text.Length / 2), rnd.Next(0, text.Length / 2));
                    }
                    catch
                    {
                        txt = "Some text here";
                    }


                    Tweet tweet = new Tweet()
                    {
                        Id = id.ToString(),
                        Stamp = Convert.ToDateTime(y + "-" + m + "-" + day + " " + DateTime.Now.ToShortTimeString())
                            .ToString(),
                        Text = txt
                    };
                    testData.Add(tweet);
                    id += rnd.Next(1, 25);
                }
            }

            return testData;
        }

        private string text = "Contrary to popular belief, Lorem Ipsum is not simply random text. It has roots in a piece of classical Latin literature from 45 BC, making it over 2000 years old. Richard McClintock, a Latin professor at Hampden-Sydney College in Virginia, looked up one of the more obscure Latin words, consectetur, from a Lorem Ipsum passage, and going through the cites of the word in classical literature, discovered the undoubtable source. Lorem Ipsum comes from sections 1.10.32 and 1.10.33 of de Finibus Bonorum et Malorum (The Extremes of Good and Evil) by Cicero, written in 45 BC.";
    }
}
