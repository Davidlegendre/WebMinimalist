using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WebBrowserMinimalist.Models
{
    public class HistoryModel
    {
        //SELECT id, url, title, visit_count, typed_count, last_visit_time, hidden
       // FROM urls
        public Int64 id { get; set; }
        public string? title { get; set; }
        public string? url { get; set; }
        public string? date { get; set; }

    }

    
}
