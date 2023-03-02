using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using WebBrowserMinimalist.Models;
using WebBrowserMinimalist.Services;
using WebBrowserMinimalist.Views.Windows;

namespace WebBrowserMinimalist.ViewModels
{
    public partial class HistoryListVM : ObservableObject
    {
        readonly HistoryServices _historyServices;
        readonly MainWindow? _mainWindow;
        readonly MensajeService _msn;
        public HistoryListVM() { 
            _historyServices = App.GetService<HistoryServices>();
            _mainWindow = App.Current.MainWindow as MainWindow;
            _msn = App.GetService<MensajeService>();
        }

        [ObservableProperty]
        ObservableCollection<HistoryModel> _listaHistorial = new ObservableCollection<HistoryModel>();

        // [RelayCommand]
        //async void ClearHistorial(HistoryModel? historyModel) {
        //    if(historyModel != null)
        //     {
        //         _historyServices?.ClearOne(historyModel);
        //         var result = await _historyServices.GetAllHistories();
        //         ListaHistorial = new ObservableCollection<HistoryModel>(result);

        //     }
        // }

        // [RelayCommand]
        //async void ClearAll() {
        //     _historyServices.ClearAll();
        //     var result = await _historyServices.GetAllHistories();
        //     ListaHistorial = new ObservableCollection<HistoryModel>(result);
        // }

        [RelayCommand]
        void openHistory() {
            if (_mainWindow != null)
            {
                var newItem = new ItemModel();
                newItem.Tab._tabItemVM.Search("edge://history/all", newItem.Tab.webview);
                _mainWindow._viewmodel.Items.Add(newItem);
                _mainWindow.lista.SelectedItem = newItem;
                _mainWindow.lista.ScrollIntoView(newItem);
            }
        }

        [RelayCommand]
        async void DeleteAll() { 
            if(_mainWindow != null)
            {
                var item = _mainWindow.lista.SelectedItem as ItemModel;
                if (item != null)
                {
                    if (_msn.ShowDialog("Desea Eliminar Todo el historial?, esto eliminara tambien las sesiones", "Eliminar Historial Data", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
                    {
                        await item.Tab.webview.CoreWebView2.Profile.ClearBrowsingDataAsync();
                        var result = await _historyServices.GetAllHistories();
                        ListaHistorial = new ObservableCollection<HistoryModel>(result);
                    }
                }
            }
        }

        [RelayCommand]
        void Navegar(HistoryModel? historyModel)
        {
            if(historyModel != null)
            {
                if (_mainWindow != null)
                {
                    var newItem = new ItemModel();
                    newItem.Tab._tabItemVM.Search(historyModel.url, newItem.Tab.webview);
                    _mainWindow._viewmodel.Items.Add(newItem);
                    _mainWindow.lista.SelectedItem = newItem;
                    _mainWindow.lista.ScrollIntoView(newItem);
                }
            }
        }

        public async void ActualizarVM() {
            var result = await _historyServices.GetAllHistories();
            ListaHistorial = new ObservableCollection<HistoryModel>(result);
        }
        
    }
}
