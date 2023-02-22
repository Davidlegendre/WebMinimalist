using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebBrowserMinimalist.Models;

namespace WebBrowserMinimalist.Services
{
    public class HistoryServices
    {
        List<HistoryModel> _historial = new List<HistoryModel>();


        public void Insert(HistoryModel historyModel)
        {
            if (historyModel.title != historyModel.url)
            {
                var result = _historial.Count(x => x.title == historyModel.title) > 0;
                if (!result)
                    _historial.Add(historyModel);
            }
        }


        public void ClearAll()
        {
           _historial.Clear();
        }

        public void ClearOne(HistoryModel? historyModel)
        {
            if (historyModel != null)
            {
               _historial.Remove(historyModel);
            }
        }

        public int CountHistory() {
            return _historial.Count();
        }
         
        public List<HistoryModel>? FindHistories(string filter)
        {
            return _historial.Where(x => x.title!.ToLower().Contains(filter.ToLower())).ToList();
        }

        public List<HistoryModel> GetAllHistories() {
            return _historial;
        }

    }
}
