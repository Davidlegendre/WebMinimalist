﻿using Microsoft.Web.WebView2.Core;
using System;
using System.Collections;
using System.Collections.Generic;
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
        readonly MainWindow? mainWindow;

        private readonly OperacionesService _operacionesService;
        readonly ItemModel ModelP;
        public TabItem(ItemModel modelP)
        {
            InitializeComponent();
            _operacionesService = App.GetService<OperacionesService>();
            mainWindow = App.Current.MainWindow as MainWindow;
            _tabItemVM = DataContext as TabItemVM;
            ModelP = modelP;
            InitialWebView2();
        }

        async void InitialWebView2() {
           await webview.EnsureCoreWebView2Async();
            webview.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
            webview.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
            webview.CoreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
            webview.CoreWebView2.IsDocumentPlayingAudioChanged += CoreWebView2_IsDocumentPlayingAudioChanged;
            webview.CoreWebView2.ServerCertificateErrorDetected += CoreWebView2_ServerCertificateErrorDetected;
            webview.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
            _tabItemVM.Search(_operacionesService.GetURlEngine().Replace("/search?q=", "").Replace("?q=", ""), webview);

        }

        private void CoreWebView2_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
           
            if(_tabItemVM != null)
            {
                e.Handled = true;
                var uri = e.Uri;
                var newItem = new ItemModel();
                //newItem.Tab._tabItemVM.se
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

                URLSearchBar.Visibility = Visibility.Collapsed;
                IndicatorDocumentPage.Visibility = Visibility.Visible;
            }
        }

        private async void CoreWebView2_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (_tabItemVM != null)
            {                
                await Task.Run(async () => await _tabItemVM.geticon(sender));
                _tabItemVM.Refreshvisibility = Visibility.Visible;
                _tabItemVM.ProgressVisibility = Visibility.Collapsed;
                _tabItemVM.UrlSource = webview.CoreWebView2.Source;
                img.Visibility = Visibility.Visible;
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

                ModelP.IMg = _tabItemVM.Image;
                ModelP.Source = _tabItemVM.UrlSource;
                ModelP.ProgressVisibility = _tabItemVM.ProgressVisibility;
                ModelP.Refreshvisibility = _tabItemVM.Refreshvisibility;
                ModelP.ShieldIcon = _tabItemVM.ShieldIcon;
            }
        }

        private void CoreWebView2_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            if (_tabItemVM != null)
            {
                _tabItemVM.Refreshvisibility = Visibility.Collapsed;
                _tabItemVM.ProgressVisibility = Visibility.Visible;
                img.Visibility = Visibility.Collapsed;

                ModelP.Source = _tabItemVM.UrlSource;
                ModelP.TitleDoc = _tabItemVM.TitleDocument;
                ModelP.ProgressVisibility = _tabItemVM.ProgressVisibility;
                ModelP.Refreshvisibility = _tabItemVM.Refreshvisibility;
            }
        }
        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Enter)
            {
                if (_tabItemVM != null)
                {
                    _tabItemVM.Search("https://www." + TxtSearch.Text + ".com", webview);
                    webview.Focus();
                }
            }
            else if (e.Key == Key.Enter)
                if (_tabItemVM != null)
                {
                    _tabItemVM.Search(TxtSearch.Text, webview);
                    webview.Focus();
                }

            

        }

        

        private void menubtn_Click(object sender, RoutedEventArgs e)
        {
            if (webview.Visibility == Visibility.Visible)
                webview.Visibility = Visibility.Collapsed;
            else
                webview.Visibility = Visibility.Visible;
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
            Settings settingsWindow = new Settings();
            settingsWindow.Owner = mainWindow;
            settingsWindow.ShowDialog();
        }
    }
}
