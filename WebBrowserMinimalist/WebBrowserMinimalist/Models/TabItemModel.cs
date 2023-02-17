using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using WebBrowserMinimalist.Services;
using WebBrowserMinimalist.ViewModels;
using Wpf.Ui.Common;
using Wpf.Ui.Mvvm.Interfaces;

namespace WebBrowserMinimalist.Models
{
    public partial class TabItemModel : ObservableObject
    {
        private readonly OperacionesService _operacionesService;
        private readonly MainWindowViewModel _mainViewModel;
        public TabItemModel(MainWindowViewModel mainWindowViewModel) {
            _operacionesService = App.GetService<OperacionesService>();
            _mainViewModel = mainWindowViewModel;
        // Search("https://www.google.com");
        }
        static Uri _DefaultUriImg => new Uri("/Assets/applicationIcon-256.png", UriKind.RelativeOrAbsolute);

        [ObservableProperty]
        string? _TitleDocument = "Blank";

        [ObservableProperty]
        BitmapImage? _ImagenUrl = new BitmapImage(_DefaultUriImg);

        [ObservableProperty]
        Visibility _ProgressVisible = Visibility.Collapsed;

        [ObservableProperty]
        string _URLNavigated = "Home";

        [ObservableProperty]
        SymbolRegular _symbolRegular = SymbolRegular.Shield16;

        [ObservableProperty]
        WebView2? _Web = null;

        public TypePage _typePage = TypePage.Blank;




        public async void Search(string url)
        {
            if (Web == null)
            {
                Web = new WebView2();
                await Web.EnsureCoreWebView2Async();
                Web.CoreWebView2.NavigationStarting += (send, obj) =>
                {
                    ProgressVisible = Visibility.Visible;
                    _typePage = TypePage.Page;
                    URLNavigated = url;
                    ChangeMain();
                };

                Web.CoreWebView2.NavigationCompleted += async (send, obj) =>
                {
                    TitleDocument = Web?.CoreWebView2.DocumentTitle;
                    if (Web.CoreWebView2.Source.StartsWith("http:")) SymbolRegular = SymbolRegular.ShieldDismiss16;
                    else SymbolRegular = SymbolRegular.Shield16;
                    await Task.Run(async () => await geticon(send));
                    ProgressVisible = Visibility.Collapsed;
                    ChangeMain();
                   
                    
                };
            }
            Web.CoreWebView2.Navigate(url);
        }

        public void ChangeMain() {
            _mainViewModel.TitleDocument = TitleDocument;
            _mainViewModel!.SymbolRegular = SymbolRegular;
            _mainViewModel.ProgressVisible = ProgressVisible;
            _mainViewModel!.ImagenUrl = ImagenUrl;
        }

        async Task geticon(object sender)
        {

            var browser = (Microsoft.Web.WebView2.Core.CoreWebView2)sender;
            Thread.Sleep(1000);
           await Application.Current.Dispatcher.Invoke(async() =>
            {
                var icon = await browser?.GetFaviconAsync(Microsoft.Web.WebView2.Core.CoreWebView2FaviconImageFormat.Png);

                ImagenUrl = _operacionesService.GetBitmap(icon);

                _mainViewModel!.ImagenUrl = ImagenUrl;
            });
        }

       

    }

   public enum TypePage
    { 
        Blank,
        Page
    }
}
