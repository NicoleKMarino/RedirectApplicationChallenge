using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedirectApplication.Models
{
    public class Response
    {
        public string RedirectUrl { get; set; }

        public int StatusCode { get; set; }

        public Response(string redirectUrl, int statusCode)
        {
            this.RedirectUrl = redirectUrl;
            this.StatusCode = statusCode;
        }
    }
}
