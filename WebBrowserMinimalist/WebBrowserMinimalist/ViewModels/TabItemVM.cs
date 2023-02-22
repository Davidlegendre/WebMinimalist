using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using WebBrowserMinimalist.Models;
using WebBrowserMinimalist.Services;
using WebBrowserMinimalist.Views.Controls;
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
            Url = new Uri(_operacionesService.GetURlEngine().Replace("/search?q=", "").Replace("?q=", ""));
        }

        [ObservableProperty]
        string _TitleDocument = "Buscando";

        [ObservableProperty]
        BitmapImage _Image = new BitmapImage(_DefaultUriImg);

        [ObservableProperty]
        string _UrlSource = "Navigating";

        [ObservableProperty]
        bool _IsNotIncognit = true;

        [ObservableProperty]
        ObservableCollection<HistoryModel>? _ResultItems = new ObservableCollection<HistoryModel>();


        [ObservableProperty]
        Uri? _Url;

        [ObservableProperty]
        Visibility _refreshvisibility = Visibility.Visible;

        [ObservableProperty]
        Visibility _SoundVisibility = Visibility.Collapsed;

        [ObservableProperty]
        SymbolRegular _ShieldIcon = SymbolRegular.Empty;


        [ObservableProperty]
        Visibility _progressVisibility = Visibility.Collapsed;

        [ObservableProperty]
        Visibility _imgVisible = Visibility.Collapsed;



        [RelayCommand]
        void Refresh(WebView2? webView2) {
            if (webView2 != null)
            {
                webView2.CoreWebView2.Reload();
                UrlSource = webView2.CoreWebView2.Source;
            }
        }

        [RelayCommand]
        void Stop(WebView2? webView2)
        {
            if (webView2 != null)
                webView2.CoreWebView2.Stop();
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
        //file:
        public void Search(string? texto) {
            
            if (texto != null && !texto.StartsWith("edge://surf")) {
                if (_operacionesService.PerteneceADominio(texto) && !Uri.IsWellFormedUriString(texto, UriKind.Absolute))
                    if (!texto.StartsWith("http:") && !texto.StartsWith("https:")
                        && !texto.StartsWith("edge:") && !texto.StartsWith("file:"))
                        texto = "https://" + texto;
                if (_operacionesService.IsArchivoAdmitido(texto) && !texto.StartsWith("http:") && !texto.StartsWith("https:"))
                {
                    if (!texto.StartsWith("file:"))
                        Url = new Uri("file:///" + texto);
                    else
                        Url = new Uri(texto);
                    UrlSource = texto;
                }
                else
                if (Uri.IsWellFormedUriString(texto, UriKind.Absolute))
                {                  
                    Url = new Uri(texto);
                    UrlSource = texto;
                }
                else
                {
                    Url = new Uri(_operacionesService.GetURlEngine() + WebUtility.UrlEncode(texto));
                    UrlSource = Url.ToString();
                }
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
                var main = App.Current.MainWindow as MainWindow;
                ProgressVisibility = Visibility.Collapsed;
                ImgVisible = Visibility.Visible;
            });
            
        }
    }
}
