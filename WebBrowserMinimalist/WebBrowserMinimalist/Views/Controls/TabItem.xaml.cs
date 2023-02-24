using Microsoft.Web.WebView2.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        WindowState _previewState;
       

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
            btnAddPage.Click += mainWindow.addbutton_Click;
            ModelP = modelP;
            InitialWebView2();
            _previewState = mainWindow.WindowState;
            
        }

        async void InitialWebView2() {
            
           await webview.EnsureCoreWebView2Async();
            
            webview.CoreWebView2.ContainsFullScreenElementChanged += CoreWebView2_ContainsFullScreenElementChanged;
            webview.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
            webview.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
            webview.CoreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
            webview.CoreWebView2.IsDocumentPlayingAudioChanged += CoreWebView2_IsDocumentPlayingAudioChanged;
            webview.CoreWebView2.ServerCertificateErrorDetected += CoreWebView2_ServerCertificateErrorDetected;
            webview.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
            if (_globalService.GetDefaulProfileFolder() == "")
                _globalService.SetDefaultProfileFolder(webview.CoreWebView2.Profile.ProfilePath);

            ModelP.IMg = _tabItemVM.Image;
            ModelP.Source = _tabItemVM.UrlSource;
            ModelP.TitleDoc = _tabItemVM.TitleDocument;
            ModelP.ShieldIcon = _tabItemVM.ShieldIcon;
        }


        private void CoreWebView2_ContainsFullScreenElementChanged(object? sender, object e)
        {
            if (mainWindow != null) {

                if (webview.CoreWebView2.ContainsFullScreenElement)
                {
                    _previewState = mainWindow.WindowState;
                    toolBar.Visibility = Visibility.Collapsed;
                    mainWindow.Visibility = Visibility.Collapsed;
                    mainWindow.WindowStyle = WindowStyle.None;
                    mainWindow.ResizeMode = ResizeMode.NoResize;
                    mainWindow.WindowState = WindowState.Maximized;
                    mainWindow._viewmodel.MarginWindowState = new Thickness(0);
                    mainWindow.Visibility = Visibility.Visible;
                    webview.Focus();
                }
                else
                {
                    toolBar.Visibility = Visibility.Visible;
                    mainWindow.WindowStyle = WindowStyle.SingleBorderWindow;
                    mainWindow.ResizeMode = ResizeMode.CanResize;
                    mainWindow.WindowState = _previewState;
                    mainWindow.Visibility = Visibility.Visible;
                    Wpf.Ui.Animations.Transitions.ApplyTransition(toolBar, Wpf.Ui.Animations.TransitionType.FadeInWithSlide, 300);
                    mainWindow._viewmodel.MarginWindowState = _previewState == WindowState.Maximized ? new Thickness(7) : new Thickness(0);
                    webview.Focus();

                }
                
                

            }
        }

        private void CoreWebView2_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
           
            if(_tabItemVM != null)
            {
                e.Handled = true;

                var uri = e.Uri;

                var newItem = new ItemModel();
                newItem.Tab._tabItemVM.Search(uri);
                newItem.Tab.countitem.DataContext = mainWindow.lista;
                mainWindow._viewmodel.Items.Add(newItem);

                mainWindow.lista.SelectedItem = newItem;
               
            }

        }

        private void CoreWebView2_ServerCertificateErrorDetected(object? sender, CoreWebView2ServerCertificateErrorDetectedEventArgs e)
        {
           if(_tabItemVM != null) {
                _tabItemVM.ShieldIcon = Wpf.Ui.Common.SymbolRegular.ShieldDismiss24;
                ModelP.ShieldIcon = _tabItemVM.ShieldIcon;
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

                _historyServices.Insert(new HistoryModel() { 
                    id = _historyServices.CountHistory() + 1,
                    title = _tabItemVM.TitleDocument,
                    url = _tabItemVM.UrlSource,
                    date = DateTime.Now.ToLongTimeString()
                });
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

                    await Task.Run(async () => await _tabItemVM.geticon(sender));                                      
                                 
                    ModelP.IMg = _tabItemVM.Image;
                    ModelP.Source = _tabItemVM.UrlSource;                    
                    ModelP.ProgressVisibility = _tabItemVM.ProgressVisibility;
                    ModelP.Refreshvisibility = _tabItemVM.Refreshvisibility;

                    
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
                if (webview.CoreWebView2.Source.Contains("edge://surf/"))
                    e.Cancel = true;
                else
                {


                    //if (_tabItemVM.UrlSource.StartsWith("http:"))
                    //{
                    //    _tabItemVM.ShieldIcon = Wpf.Ui.Common.SymbolRegular.ShieldError24;
                    //}
                    //else if (_tabItemVM.UrlSource.StartsWith("https:"))
                    //    _tabItemVM.ShieldIcon = Wpf.Ui.Common.SymbolRegular.Shield24;
                    _tabItemVM.ShieldIcon = Wpf.Ui.Common.SymbolRegular.Empty;


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
        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Enter)
            {
                if (_tabItemVM != null)
                {
                    _tabItemVM.Search("https://www." + TxtSearch.Text + ".com");
                    webview.Focus();
                }
            }
            else if (e.Key == Key.Enter)
                if (_tabItemVM != null)
                {
                    _tabItemVM.Search(TxtSearch.Text);
                    webview.Focus();
                }
        }

        

        private async void menubtn_Click(object sender, RoutedEventArgs e)
        {
            if (!mainWindow.flyoutPanel.IsOpen)
            {
                //mainWindow.optionsBrowser.Visibility = Visibility.Visible;
                mainWindow.flyoutPanel.IsOpen = true;
                mainWindow.lista.Visibility = Visibility.Visible;
                mainWindow.historyList.Visibility = Visibility.Collapsed;
                mainWindow.btnAtras.Visibility = Visibility.Collapsed;                
                Wpf.Ui.Animations.Transitions.ApplyTransition(mainWindow.flyoutPanel,
                Wpf.Ui.Animations.TransitionType.FadeInWithSlide, 400);
                mainWindow.historyList.Actualizar();
            }
        }

        private void btnChangeToSearch_Click(object sender, RoutedEventArgs e)
        {
            IndicatorDocumentPage.Visibility = Visibility.Collapsed;
            URLSearchBar.Visibility= Visibility.Visible;
        }

        private void ReturnToIndicator_Click(object sender, RoutedEventArgs e)
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

            var Source = _tabItemVM.UrlSource;
           

            if (Source.Contains("http:") || Source.Contains("https:"))
            {
                _tabItemVM.ShieldIcon = _errorPageService.GetSymbol(errorStatus);
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
                    _tabItemVM.ResultItems = new System.Collections.ObjectModel.ObservableCollection<HistoryModel>(result);
                }
                else
                {
                    flyResults.IsOpen = false;
                    _tabItemVM.ResultItems = new System.Collections.ObjectModel.ObservableCollection<HistoryModel>();
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
                _tabItemVM.UrlSource = item.url;
                TxtSearch.Focus();
                TxtSearch.SelectAll();
                flyResults.IsOpen = false;
            }
        }

        private void webview_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            if (_tabItemVM.UrlSource != "Home")
                webview.Visibility = Visibility.Visible;
        }
    }
}
