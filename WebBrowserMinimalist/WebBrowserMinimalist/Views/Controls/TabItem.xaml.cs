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
        WindowState _previewState;

        readonly ItemModel ModelP;
        public TabItem(ItemModel modelP, bool? IsIncognite = true)
        {
            InitializeComponent();
            mainWindow = App.Current.MainWindow as MainWindow;
            _tabItemVM = DataContext as TabItemVM;
            _historyServices = App.GetService<HistoryServices>();
            ModelP = modelP;
            InitialWebView2();
            webview.CreationProperties = new Microsoft.Web.WebView2.Wpf.CoreWebView2CreationProperties() { IsInPrivateModeEnabled = IsIncognite };
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
                    TitleDocument= _tabItemVM.TitleDocument,
                    URL = _tabItemVM.UrlSource
                });
            }
        }

        private async void CoreWebView2_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (_tabItemVM != null)
            {

                try
                {
                    await Task.Run(async () => await _tabItemVM.geticon(sender));
                    _tabItemVM.ProgressVisibility = Visibility.Collapsed;
                    progressring.IsIndeterminate = false;
                    _tabItemVM.UrlSource = webview.CoreWebView2.Source;
                    btnRefresh.Visibility = Visibility.Visible;
                    btnStop.Visibility = Visibility.Collapsed;
                    img.Visibility = Visibility.Visible;
                    CambiarIconoShield();

                    ModelP.IMg = _tabItemVM.Image;
                    ModelP.Source = _tabItemVM.UrlSource;                    
                    ModelP.ProgressVisibility = _tabItemVM.ProgressVisibility;
                    ModelP.Refreshvisibility = _tabItemVM.Refreshvisibility;
                    ModelP.ShieldIcon = _tabItemVM.ShieldIcon;
                    mainWindow.historyList.Actualizar();
                    webview.Focus();
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
                //var ruta = "C:/Users/" + Environment.UserName + "/AppData/Local/Microsoft/Edge/User Data/Default/Extensions";
                //var exist = Directory.Exists(ruta);
                //if (exist)
                //{
                //    //adblock
                //    var archivo = ruta + "/gmgoamodcdcjnbaobigkjelfplakmdhh/3.16.1_0/vendor/webext-sdk/content.js";
                //    if (File.Exists(archivo))
                //    {
                //        var js = await File.ReadAllTextAsync(archivo);
                //        await webview.ExecuteScriptAsync(js);
                //    }
                //    //adblock youtube
                //}

                _tabItemVM.ShieldIcon = Wpf.Ui.Common.SymbolRegular.Empty;

                btnRefresh.Visibility= Visibility.Collapsed;
                btnStop.Visibility= Visibility.Visible;
                _tabItemVM.ProgressVisibility = Visibility.Visible;
                progressring.IsIndeterminate = true;
                img.Visibility = Visibility.Collapsed;

                ModelP.Source = _tabItemVM.UrlSource;
                ModelP.TitleDoc = _tabItemVM.TitleDocument;
                ModelP.ProgressVisibility = _tabItemVM.ProgressVisibility;
                ModelP.ShieldIcon = _tabItemVM.ShieldIcon;

                flyResults.IsOpen = false;
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

        

        private void menubtn_Click(object sender, RoutedEventArgs e)
        {
            if (!mainWindow.flyoutPanel.IsOpen)
            {
                //mainWindow.optionsBrowser.Visibility = Visibility.Visible;
                mainWindow.flyoutPanel.Show();
                mainWindow.lista.Visibility = Visibility.Visible;
                mainWindow.historyList.Visibility = Visibility.Collapsed;
                mainWindow.historyList.Actualizar();
                Wpf.Ui.Animations.Transitions.ApplyTransition(mainWindow.flyoutPanel,
                Wpf.Ui.Animations.TransitionType.FadeInWithSlide, 400);
            }
            else
            {
                mainWindow.flyoutPanel.Hide();
                //mainWindow.optionsBrowser.Visibility = Visibility.Collapsed;
               
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
            { FlyoutSettingPanel.Show(); }
            else { FlyoutSettingPanel.Hide(); }
        }

        void CambiarIconoShield() {
            if (webview.CoreWebView2.Source.Contains("https:"))
                _tabItemVM.ShieldIcon = Wpf.Ui.Common.SymbolRegular.Shield24;
            else
                _tabItemVM.ShieldIcon = Wpf.Ui.Common.SymbolRegular.ShieldDismiss24;

            if (webview.CoreWebView2.Source.Contains("file:"))
                _tabItemVM.ShieldIcon = Wpf.Ui.Common.SymbolRegular.Archive24;
            if (webview.CoreWebView2.Source == "edge://downloads/all")
                _tabItemVM.ShieldIcon = Wpf.Ui.Common.SymbolRegular.ArrowDownload24;
            if (webview.CoreWebView2.Source == "edge://history/all")
                _tabItemVM.ShieldIcon = Wpf.Ui.Common.SymbolRegular.History24;
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
            }
            else if (flyResults != null)
                flyResults.IsOpen = false;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (HistoryModel)listResultSearch.SelectedItem;
            if (item != null)
            {
                _tabItemVM.UrlSource = item.URL;
                TxtSearch.Focus();
                TxtSearch.SelectAll();
                flyResults.IsOpen = false;
            }
        }
    }
}
