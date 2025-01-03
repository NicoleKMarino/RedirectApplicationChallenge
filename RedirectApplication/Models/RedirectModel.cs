using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedirectApplication.Models
{
    public class RedirectModel
    {
        public string RedirectUrl { get; set; }

        public string TargetUrl { get; set; }

        public int RedirectType { get; set; }

        public bool UseRelative { get; set; }

        public RedirectModel()
        {
        }
    }
}
