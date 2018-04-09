using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TweetBrowser.Models
{
    public class Tweet
    {
        [Display(Name="ID")]
        public string Id { get; set; }
        [Display(Name = "Date/Time")]
        public string Stamp { get; set; }
        [Display(Name = "Message")]
        public string Text { get; set; }

        // Converts the date string to a typed date.
        // If the date string is invalid it returns a default (min date value).
        public DateTime Date
        {
            get
            {
                DateTime date;
                try
                {
                    date = Convert.ToDateTime(Stamp);
                }
                catch
                {
                    return DateTime.MinValue;
                }

                return date;
            }
        }
    }
}
