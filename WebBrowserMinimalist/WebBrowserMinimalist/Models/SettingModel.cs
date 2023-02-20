using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBrowserMinimalist.Services;

namespace WebBrowserMinimalist.Models
{
    public class SettingModel
    {
        public TypeSearchEngine typeSearchEngine { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}
