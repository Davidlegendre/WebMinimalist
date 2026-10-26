using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WebBrowserMinimalist.Models;
using WebBrowserMinimalist.Services;
using WebBrowserMinimalist.Views.Windows;

namespace WebBrowserMinimalist.ViewModels
{
    public partial class HistoryListVM : ObservableObject
    {
        private readonly HistoryServices _historyServices;
        private readonly MainWindow? _mainWindow;

        public HistoryListVM()
        {
            _historyServices = App.GetService<HistoryServices>()!;
            _mainWindow = App.Current.MainWindow as MainWindow;
        }

        [ObservableProperty]
        private ObservableCollection<HistoryModel> _listaHistorial = new();

        public async Task ClearHistoryAsync(
            CoreWebView2Profile profile,
            DateTime? startUtc,
            DateTime? endUtc,
            bool clearSiteData)
        {
            if (startUtc.HasValue != endUtc.HasValue)
                throw new ArgumentException("Both range endpoints must be provided together.");

            var dataKinds = CoreWebView2BrowsingDataKinds.BrowsingHistory;
            if (clearSiteData)
                dataKinds |= CoreWebView2BrowsingDataKinds.AllSite;

            try
            {
                if (startUtc.HasValue)
                    await profile.ClearBrowsingDataAsync(dataKinds, startUtc.Value, endUtc!.Value);
                else
                    await profile.ClearBrowsingDataAsync(dataKinds);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(
                    "No se pudo borrar el historial del navegador WebView2.", exception);
            }

            try
            {
                if (startUtc.HasValue)
                    await _historyServices.ClearBetweenAsync(startUtc.Value, endUtc!.Value);
                else
                    await _historyServices.ClearAllAsync();
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(
                    "WebView2 se limpió, pero no se pudo borrar el historial de la aplicación.", exception);
            }

            var history = await _historyServices.GetAllHistoriesAsync();
            ListaHistorial = new ObservableCollection<HistoryModel>(history);
        }

        [RelayCommand]
        private void Navegar(HistoryModel? historyModel)
        {
            if (historyModel?.url == null || _mainWindow == null)
                return;

            var newItem = new ItemModel();
            newItem.Tab!._tabItemVM!.Search(historyModel.url, newItem.Tab.webview);
            _mainWindow._viewmodel!.Items!.Add(newItem);
            _mainWindow.lista.SelectedItem = newItem;
            _mainWindow.lista.ScrollIntoView(newItem);
        }

        public async void ActualizarVM()
        {
            var result = await _historyServices.GetAllHistoriesAsync();
            ListaHistorial = new ObservableCollection<HistoryModel>(result);
        }
    }
}
