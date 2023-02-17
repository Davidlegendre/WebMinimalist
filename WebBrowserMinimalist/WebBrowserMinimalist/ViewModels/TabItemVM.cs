using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using WebBrowserMinimalist.Services;
using WebBrowserMinimalist.Views.Windows;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Mvvm.Interfaces;

namespace WebBrowserMinimalist.ViewModels
{
    public partial class TabItemVM : ObservableObject
    {

        static Uri _DefaultUriImg => new Uri("/Assets/applicationIcon-256.png", UriKind.RelativeOrAbsolute);

        private readonly OperacionesService _operacionesService;
        public TabItemVM()
        {
            _operacionesService = App.GetService<OperacionesService>();
        }

        [ObservableProperty]
        string _TitleDocument = "Buscando";

        [ObservableProperty]
        BitmapImage _Image = new BitmapImage(_DefaultUriImg);

        [ObservableProperty]
        string _UrlSource = "Navigating";

        [ObservableProperty]
        Visibility _refreshvisibility = Visibility.Visible;

        [ObservableProperty]
        Visibility _SoundVisibility = Visibility.Collapsed;

        [ObservableProperty]
        SymbolRegular _ShieldIcon = SymbolRegular.Shield24;

        [ObservableProperty]
        Uri _UrL = new Uri("https://www.google.com");

        [ObservableProperty]
        Visibility _progressVisibility = Visibility.Collapsed;

        

        [RelayCommand]
        void Refresh(WebView2? webView2) {
            if (webView2 != null)
            {
                webView2.CoreWebView2.Reload();               
            }        
        }

        [RelayCommand]
        void Back(WebView2? webView2)
        {
            if (webView2 != null)
            {
                webView2.CoreWebView2.GoBack();
            }
        }
        
        [RelayCommand]
        void Left(WebView2? webView2)
        {
            if (webView2 != null)
            {
                webView2.CoreWebView2.GoForward();
            }
        }

        [RelayCommand]
        void Search(string texto) {
            if (texto != null) {
                texto = !Uri.IsWellFormedUriString(texto, UriKind.RelativeOrAbsolute) ? 
                    texto : "https://www.bing.com/search?q=" + texto.Replace(" ", "+");

                UrL = new Uri(texto);
            }
        }

       

       public async Task geticon(object sender)
        {

            var browser = (Microsoft.Web.WebView2.Core.CoreWebView2)sender;
            Thread.Sleep(1000);
            await Application.Current.Dispatcher.Invoke(async () =>
            {
                var icon = await browser?.GetFaviconAsync(Microsoft.Web.WebView2.Core.CoreWebView2FaviconImageFormat.Png);

                Image = _operacionesService.GetBitmap(icon);
            });
        }



    }
}
