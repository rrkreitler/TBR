using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TweetBrowser.Pages
{
    public class ErrorModel : PageModel
    {
        public string Message { get; set; }

        public void OnGet(string message)
        {
            if (String.IsNullOrWhiteSpace(message))
            {
                message = "Sorry, there was a problem processing that request.";
            }
            Message = message;
        }

    }
}
