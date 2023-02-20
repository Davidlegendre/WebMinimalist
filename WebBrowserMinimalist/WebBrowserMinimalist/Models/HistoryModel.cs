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
        public Int64 id { get; set; }
        public string? TitleDocument { get; set; }
        public string? URL { get; set; }
    }

    
}
