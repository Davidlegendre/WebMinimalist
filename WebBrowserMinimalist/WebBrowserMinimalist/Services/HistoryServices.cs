using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebBrowserMinimalist.DBA;
using WebBrowserMinimalist.Models;

namespace WebBrowserMinimalist.Services
{
    public class HistoryServices
    {
        readonly HIstorialDBA _history;
        List<HistoryModel> _historial = new List<HistoryModel>();
        public HistoryServices(HIstorialDBA history)
        {
            _history = history;
        }

        //public void ClearAll()
        //{
        //   _historial.Clear();
        //   _history.DeleteAll();
        //}

        //public void ClearOne(HistoryModel? historyModel)
        //{
        //    if (historyModel != null)
        //    {
        //       _historial.Remove(historyModel);
        //        _history.deleteOne(historyModel);
        //    }
        //}

        public int CountHistory() {
            return _historial.Count();
        }
         
        public List<HistoryModel>? FindHistories(string filter)
        {
            return _historial.Where(x => x.title!.ToLower().Contains(filter.ToLower())).ToList();
        }

        public async Task<List<HistoryModel>> GetAllHistories() {
            var result =await  _history.ObtenerDatos();
            _historial = result.ToList();
            return _historial;
        }

    }
}
