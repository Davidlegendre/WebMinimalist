using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WebBrowserMinimalist.Models;
using WebBrowserMinimalist.Services;
using WebBrowserMinimalist.ViewModels;
using WebBrowserMinimalist.Views.Windows;
using Wpf.Ui.Mvvm.Interfaces;

namespace WebBrowserMinimalist.Views.Controls
{
    /// <summary>
    /// Lógica de interacción para TabItem.xaml
    /// </summary>
    public partial class TabItem : UserControl
    {
        public readonly TabItemVM? _tabItemVM;
        readonly HistoryServices? _historyServices;
        readonly MainWindow? mainWindow;
        readonly ErrorsPageService? _errorPageService;
        readonly MensajeService _msn;
        WindowState _previewState;

        readonly AdblockService _adblock;

        readonly GlobalService _globalService;

        readonly ItemModel ModelP;
        public TabItem(ItemModel modelP)
        {
            InitializeComponent();
            mainWindow = App.Current.MainWindow as MainWindow;
            _tabItemVM = DataContext as TabItemVM;
            _historyServices = App.GetService<HistoryServices>();
            _globalService = App.GetService<GlobalService>();
            _errorPageService = App.GetService<ErrorsPageService>();
            _adblock = App.GetService<AdblockService>();
            _msn = App.GetService<MensajeService>();
           

            btnAddPage.Click += mainWindow!.addbutton_Click;
            btnCloseTab.Click += mainWindow!.btnClose_Click;
            txtCountTabs.DataContext = mainWindow!.lista;
            _previewState = mainWindow.WindowState;
            
            InitialWebView2();
            ModelP = modelP;
        }
        async void InitialWebView2() {
            //if (_globalService.IsInPrivateMode)
            //{
            //    var options = new CoreWebView2EnvironmentOptions("-inprivate");
            //    var webView2Environment = await CoreWebView2Environment.CreateAsync(options: options);
            //    webview.CreationProperties.IsInPrivateModeEnabled = true;
            //    _msn.ShowDialog("pase");
            //    await webview.EnsureCoreWebView2Async(webView2Environment);                
            //}
            //else
            //{
              
            //}
            await webview.EnsureCoreWebView2Async();
            //Binding bind = new Binding();
            //bind.Source = _tabItemVM;
            //bind.Path = new PropertyPath("Url");
            //webview.SetBinding(WebView2.SourceProperty, bind);

            webview.CoreWebView2.ContainsFullScreenElementChanged += CoreWebView2_ContainsFullScreenElementChanged;
            webview.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
            webview.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
            webview.CoreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
            webview.CoreWebView2.IsDocumentPlayingAudioChanged += CoreWebView2_IsDocumentPlayingAudioChanged;
            webview.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
             webview.CoreWebView2.PermissionRequested += CoreWebView2_PermissionRequested;
            webview.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
            webview.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
           
            
            ModelP.IMg = _tabItemVM!.Image;
            ModelP.Source = _tabItemVM.UrlSource;
            ModelP.TitleDoc = _tabItemVM.TitleDocument;
            ModelP.ShieldIcon = _tabItemVM.ShieldIcon;
        }

        #region WebviewEvents

        private void CoreWebView2_WebResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            _adblock.BlockYoutubeAds(webview, e);
            _adblock.BlockAds(webview, e);
           
        }

        private void CoreWebView2_PermissionRequested(object? sender, CoreWebView2PermissionRequestedEventArgs e)
        {
            var icon = _tabItemVM!.PermisosIcons.GetValueOrDefault(e.PermissionKind);
            if (e.State == CoreWebView2PermissionState.Deny)
                _tabItemVM.Permisos.Remove(icon);
            else if (!_tabItemVM.Permisos.Contains(icon))
            {
                _tabItemVM.Permisos.Add(icon);
            }

            //if (e.IsUserInitiated)
            //    if (!_tabItemVM.Permisos.Contains(icon))
            //    {
            //        _tabItemVM.Permisos.Add(icon);
            //    }
            //else if (!e.IsUserInitiated)
            //    _tabItemVM.Permisos.Remove(icon);


        }

        private void CoreWebView2_ContainsFullScreenElementChanged(object? sender, object e)
        {
            if (mainWindow != null) {

                if (webview.CoreWebView2.ContainsFullScreenElement)
                {
                    _previewState = mainWindow.WindowState;
                    toolBar.Visibility = Visibility.Collapsed;                   
                    mainWindow.WindowState = WindowState.Maximized;
                    mainWindow.ResizeMode = ResizeMode.NoResize;
                    webview.Margin = new Thickness(0);
                    mainWindow.titlebar.CanMaximize = false;
                    webview.Focus();
                }
                else
                {
                    toolBar.Visibility = Visibility.Visible;
                    mainWindow.titlebar.CanMaximize = true;
                    mainWindow.ResizeMode = ResizeMode.CanResize;
                    mainWindow.WindowState = _previewState;
                    webview.Margin = new Thickness(7,2,7,7);
                    Wpf.Ui.Animations.Transitions.ApplyTransition(toolBar, Wpf.Ui.Animations.TransitionType.FadeInWithSlide, 300);
                    webview.Focus();
                }
            }
        }

        private void CoreWebView2_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            if (_tabItemVM != null)
            {

                e.Handled = true;
                var uri = e.Uri;
                var newItem = new ItemModel();
                newItem.Tab._tabItemVM.Search(uri, webview);
                mainWindow._viewmodel.Items.Add(newItem);
                mainWindow.lista.SelectedItem = newItem;
            }
        }

        private void CoreWebView2_IsDocumentPlayingAudioChanged(object? sender, object e)
        {
            if (_tabItemVM != null)
            {
                if (webview.CoreWebView2.IsDocumentPlayingAudio)
                    _tabItemVM.SoundVisibility = Visibility.Visible;
                else
                    _tabItemVM.SoundVisibility = Visibility.Collapsed;

                ModelP.SoundVisibility = _tabItemVM.SoundVisibility;
            }
        }

        private void CoreWebView2_DocumentTitleChanged(object? sender, object e)
        {
            if (_tabItemVM != null)
            {
                _tabItemVM.TitleDocument = webview.CoreWebView2.DocumentTitle;
               
                ModelP.TitleDoc = _tabItemVM.TitleDocument;

                _tabItemVM.UrlSource = webview.CoreWebView2.Source;
                ModelP.Source = _tabItemVM.UrlSource;

                URLSearchBar.Visibility = Visibility.Collapsed;
                IndicatorDocumentPage.Visibility = Visibility.Visible;

                
            }
        }

        private async void CoreWebView2_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (_tabItemVM != null)
            {
                try
                {
                    
                    changePage(e.WebErrorStatus);
                    CambiarIconoShield(e.WebErrorStatus);
                    _tabItemVM.UrlSource = webview.CoreWebView2.Source;
                    btnRefresh.Visibility = Visibility.Visible;
                    btnStop.Visibility = Visibility.Collapsed;
                    ModelP.Source = _tabItemVM.UrlSource;
                    ModelP.Refreshvisibility = _tabItemVM.Refreshvisibility;
                    webview.Focus();
                    if (_tabItemVM.UrlSource.Contains("https://sg.sdasystems.org"))
                    {
                        var functionString = "var link = document.createElement('link'); link.rel = 'stylesheet'; link.type = 'text/css';  link.href = 'https://hyms-front.vercel.app/img/main.css'; document.getElementsByTagName('head')[0].appendChild(link); ";
                        //var script = "alert('hola')";
                        var result = await webview.ExecuteScriptAsync(functionString);
                    }
                    await Task.Run(async () => await _tabItemVM.geticon(sender!));
                    if (_tabItemVM.UrlSource == "about:blank")
                    {
                        _tabItemVM.Image = new BitmapImage(new Uri("/Views/Windows/icons8-internet-48.png", UriKind.RelativeOrAbsolute));
                        //webview.Visibility = Visibility.Collapsed;
                        _tabItemVM.ShieldIcon = Wpf.Ui.Common.SymbolRegular.Home12;
                        webview.DefaultBackgroundColor = System.Drawing.Color.Transparent;
                        ModelP.ShieldIcon = _tabItemVM.ShieldIcon;
                        _tabItemVM.TitleDocument = "Home";
                        home.Visibility = Visibility.Visible;
                        Wpf.Ui.Animations.Transitions.ApplyTransition(home, Wpf.Ui.Animations.TransitionType.FadeIn, 400);
                    }

                    ModelP.TitleDoc = _tabItemVM.TitleDocument;
                    ModelP.IMg = _tabItemVM.Image;
                    ModelP.Source = _tabItemVM.UrlSource;
                    ModelP.ProgressVisibility = _tabItemVM.ProgressVisibility;
                    ModelP.Refreshvisibility = _tabItemVM.Refreshvisibility;
                    //mainWindow.historyList.Actualizar();

                }
                catch (Exception)
                {
                    progressring.IsIndeterminate = false;
                }
            }
        }

        private void CoreWebView2_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            if (_tabItemVM != null)
            {

                if (_tabItemVM.UrlSource.StartsWith("edge:") && !_tabItemVM.UrlSource.Contains("edge://downloads/all") && 
                    !_tabItemVM.UrlSource.Contains("edge://history/all"))
                {
                    e.Cancel = true;
                }
                else
                if (_adblock.BlockSites(_tabItemVM.UrlSource))
                {
                    e.Cancel = true;
                }
                else
                {
                    if (_tabItemVM.UrlSource != "about:blank")
                    {
                        webview.DefaultBackgroundColor = System.Drawing.Color.White;
                        home.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        webview.DefaultBackgroundColor = System.Drawing.Color.Transparent;
                    }

                    
               


                    btnRefresh.Visibility = Visibility.Collapsed;
                    btnStop.Visibility = Visibility.Visible;
                    _tabItemVM.ProgressVisibility = Visibility.Visible;
                    _tabItemVM.ImgVisible = Visibility.Collapsed;

                    ModelP.Source = _tabItemVM.UrlSource;
                    ModelP.TitleDoc = _tabItemVM.TitleDocument;
                    ModelP.ProgressVisibility = _tabItemVM.ProgressVisibility;
                    ModelP.ShieldIcon = _tabItemVM.ShieldIcon;

                    flyResults.IsOpen = false;
                }
            }
        }

        #endregion

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Enter)
            {
                if (_tabItemVM != null)
                {
                    _tabItemVM.Search("https://www." + TxtSearch.Text + ".com", webview);
                    webview.Focus();
                    cambiarAIndicador();
                }               
            }
            else if (e.Key == Key.Enter)
                if (_tabItemVM != null)
                {
                    _tabItemVM.Search(TxtSearch.Text, webview);
                    webview.Focus();
                    cambiarAIndicador();
                }

            
        }

        

        private void menubtn_Click(object sender, RoutedEventArgs e)
        {
            if (mainWindow!.menutabs.Visibility == Visibility.Collapsed)
            {
                //mainWindow.optionsBrowser.Visibility = Visibility.Visible;
                mainWindow.menutabs.Visibility = Visibility.Visible;
                mainWindow.lista.Visibility = Visibility.Visible;
                //mainWindow.historyList.Visibility = Visibility.Collapsed;
                //mainWindow.btnAtras.Visibility = Visibility.Collapsed;                
                Wpf.Ui.Animations.Transitions.ApplyTransition(mainWindow.menutabs,
                Wpf.Ui.Animations.TransitionType.FadeIn, 400);
                //mainWindow.historyList.Actualizar();
            }
        }

        private void ReturnToIndicator_Click(object sender, RoutedEventArgs e)
        {
            cambiarAIndicador();
        }

        void cambiarAIndicador()
        {
            URLSearchBar.Visibility = Visibility.Collapsed;
            IndicatorDocumentPage.Visibility = Visibility.Visible;
        }

        private void TxtSearch_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TxtSearch.SelectAll();
        }

        private void btnSettingEngines_Click(object sender, RoutedEventArgs e)
        {
            if (!FlyoutSettingPanel.IsOpen)
            { FlyoutSettingPanel.IsOpen = true; }
        }

        void CambiarIconoShield(CoreWebView2WebErrorStatus errorStatus) {

            var Source = _tabItemVM!.UrlSource;
           

            if (Source.Contains("http:") || Source.Contains("https:"))
            {
                _tabItemVM.ShieldIcon = _errorPageService!.GetSymbol(errorStatus);
            }
            else
            {

                if (Source.Contains("file:"))
                    _tabItemVM.ShieldIcon = Wpf.Ui.Common.SymbolRegular.Archive24;
                if (Source == "edge://downloads/all")
                    _tabItemVM.ShieldIcon = Wpf.Ui.Common.SymbolRegular.ArrowDownload24;
                if (Source == "edge://history/all")
                    _tabItemVM.ShieldIcon = Wpf.Ui.Common.SymbolRegular.History24;
            }
            ModelP.ShieldIcon = _tabItemVM.ShieldIcon;
            
        }

       async void changePage(CoreWebView2WebErrorStatus? errorStatus = null) {
            if (errorStatus != null)
            {
                if (errorStatus == CoreWebView2WebErrorStatus.Disconnected)
                    await webview.CoreWebView2.ExecuteScriptAsync(@"
                   document.getElementById('game-buttons').remove()
                ");

                if (_tabItemVM.UrlSource.Contains("edge://history"))
                {
                    await webview.CoreWebView2.ExecuteScriptAsync(@"
                   document.getElementById('clear-browsing-data').remove()
                ");
                    //clear-browsing-data
                }

            }
            
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TxtSearch.Text) && _historyServices != null)
            {
                var result = _historyServices.FindHistories(TxtSearch.Text);
                if (result?.Count > 0)
                {
                    flyResults.IsOpen = true;
                    _tabItemVM!.ResultItems = new System.Collections.ObjectModel.ObservableCollection<HistoryModel>(result);
                }
                else
                {
                    flyResults.IsOpen = false;
                    _tabItemVM!.ResultItems = new System.Collections.ObjectModel.ObservableCollection<HistoryModel>();
                }
            }
            else if (flyResults != null)
                flyResults.IsOpen = false;
        }


        private void btnItemResultSearch_Click(object sender, RoutedEventArgs e)
        {

            var button = (Wpf.Ui.Controls.Button)sender;
            var item = (HistoryModel)button.DataContext;
            if (item != null)
            {               
                _tabItemVM!.Search(item.url, webview);
            }
        }
        long LastTicks = 0;

        private void TbTitleDocument_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((DateTime.Now.Ticks - LastTicks) < 3000000)
            {
                IndicatorDocumentPage.Visibility = Visibility.Collapsed;
                URLSearchBar.Visibility = Visibility.Visible;
            }
            LastTicks = DateTime.Now.Ticks;
        }

        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
           
            if (!webview.CoreWebView2.IsDefaultDownloadDialogOpen)
            {
                webview.CoreWebView2.OpenDefaultDownloadDialog();
            }
            else
            {
                webview.CoreWebView2.CloseDefaultDownloadDialog();
            }
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            if (webview.CoreWebView2 != null)
                webview.CoreWebView2.CloseDefaultDownloadDialog();
        }
    }
}
