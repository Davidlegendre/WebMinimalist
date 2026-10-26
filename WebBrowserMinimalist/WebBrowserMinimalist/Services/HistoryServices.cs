using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBrowserMinimalist.DBA;
using WebBrowserMinimalist.Models;

namespace WebBrowserMinimalist.Services
{
    public sealed class HistoryServices
    {
        private readonly HIstorialDBA _history;
        private List<HistoryModel> _historial = new();

        public HistoryServices(HIstorialDBA history)
        {
            _history = history;
        }

        public int CountHistory() => _historial.Count;

        public List<HistoryModel> FindHistories(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
                return new List<HistoryModel>();

            return _historial
                .Where(item => (item.title?.Contains(filter, StringComparison.OrdinalIgnoreCase) ?? false)
                    || (item.url?.Contains(filter, StringComparison.OrdinalIgnoreCase) ?? false))
                .Take(10)
                .ToList();
        }

        public async Task RecordVisitAsync(string url, string title)
        {
            await _history.RecordVisitAsync(url, title);
            await RefreshCacheAsync();
        }

        public async Task<List<HistoryModel>> GetAllHistoriesAsync()
        {
            await RefreshCacheAsync();
            return _historial;
        }

        public async Task ClearAllAsync()
        {
            await _history.DeleteAllAsync();
            _historial.Clear();
        }

        public async Task ClearBetweenAsync(DateTime startUtc, DateTime endUtc)
        {
            await _history.DeleteBetweenAsync(startUtc, endUtc);
            await RefreshCacheAsync();
        }

        private async Task RefreshCacheAsync()
        {
            _historial = (await _history.GetRecentAsync()).ToList();
        }
    }
}
