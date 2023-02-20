using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBrowserMinimalist.Models;

namespace WebBrowserMinimalist.Services
{
    public class HistoryServices
    {
        List<HistoryModel> _historial = new List<HistoryModel>();
        public void Insert(HistoryModel model)
        {
            _historial.Add(model);
        }

        public void ClearAll() {
            _historial.Clear();
        }

        public void ClearOne(HistoryModel? historyModel)
        {
            if (historyModel != null)
                _historial.Remove(historyModel);
        }

        public int CountHistory() {
            return _historial.Count();
        }

        public HistoryModel? GetHistory(Int64 id)
        {
            return _historial.FirstOrDefault(x => x.id == id);
        }

        public List<HistoryModel>? FindHistories(string filter)
        {
            return _historial.Where(x => x.TitleDocument!.ToLower().Contains(filter.ToLower())).Distinct().ToList();
        }

        public List<HistoryModel> GetAllHistories() {
            return _historial;
        }

    }
}
