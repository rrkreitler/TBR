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
        public string RequestId { get; set; }
        public string Message { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public void OnGet(string message)
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            Message = message;
        }
    }
}
