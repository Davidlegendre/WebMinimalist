using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using WebBrowserMinimalist.Models;
using WebBrowserMinimalist.Services;
using WebBrowserMinimalist.Views.Windows;

namespace WebBrowserMinimalist.ViewModels
{
    public partial class HistoryListVM : ObservableObject
    {
        private readonly HistoryServices _historyServices;
        private readonly MainWindow? _mainWindow;
        private readonly MensajeService _msn;

        public HistoryListVM()
        {
            _historyServices = App.GetService<HistoryServices>()!;
            _mainWindow = App.Current.MainWindow as MainWindow;
            _msn = App.GetService<MensajeService>()!;
        }

        [ObservableProperty]
        private ObservableCollection<HistoryModel> _listaHistorial = new();

        [RelayCommand]
        private async void DeleteAll()
        {
            if (_msn.ShowDialog("Desea eliminar todo el historial guardado por la aplicación?", "Eliminar historial", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            await _historyServices.ClearAllAsync();
            ListaHistorial.Clear();
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
