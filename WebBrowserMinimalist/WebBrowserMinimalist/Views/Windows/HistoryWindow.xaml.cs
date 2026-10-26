using Microsoft.Web.WebView2.Core;
using System.Windows;
using Wpf.Ui.Controls;

namespace WebBrowserMinimalist.Views.Windows
{
    public partial class HistoryWindow : UiWindow
    {
        public HistoryWindow(CoreWebView2Profile profile)
        {
            InitializeComponent();
            historyList.SetBrowserProfile(profile);
        }

        public void Actualizar()
        {
            historyList.Actualizar();
        }
    }
}
