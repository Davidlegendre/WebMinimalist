using Microsoft.Web.WebView2.Core;
using System.Threading.Tasks;

namespace WebBrowserMinimalist.Services
{
    public sealed class BrowserEnvironmentService
    {
        private readonly object _environmentLock = new();
        private Task<CoreWebView2Environment>? _environmentTask;

        public Task<CoreWebView2Environment> GetEnvironmentAsync()
        {
            lock (_environmentLock)
            {
                _environmentTask ??= CreateEnvironmentAsync();
                return _environmentTask;
            }
        }

        private static Task<CoreWebView2Environment> CreateEnvironmentAsync()
        {
            var options = new CoreWebView2EnvironmentOptions
            {
                AreBrowserExtensionsEnabled = true
            };

            return CoreWebView2Environment.CreateAsync(null, null, options);
        }
    }
}
