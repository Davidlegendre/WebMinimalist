using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBrowserMinimalist.Models
{   
    public class SitesBlockModel
    {
        public List<Site> sites { get; set; }
        public int created_time { get; set; }
    }

    public class Site
    {
        public string url { get; set; }
    }
}
