using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using WebBrowserMinimalist.Models;
using WebBrowserMinimalist.Services;
using WebBrowserMinimalist.Views.Windows;
using Wpf.Ui.Mvvm.Interfaces;

namespace WebBrowserMinimalist.ViewModels
{
    public partial class HistoryListVM : ObservableObject
    {
        readonly HistoryServices _historyServices;
        readonly MainWindow? _mainWindow;
        public HistoryListVM() { 
            _historyServices = App.GetService<HistoryServices>();
            _mainWindow = App.Current.MainWindow as MainWindow;
        }

        [ObservableProperty]
        ObservableCollection<HistoryModel> _listaHistorial = new ObservableCollection<HistoryModel>();

        [RelayCommand]
        void ClearHistorial(HistoryModel? historyModel) {
           if(historyModel != null)
            {
                _historyServices?.ClearOne(historyModel);
                ListaHistorial = new ObservableCollection<HistoryModel>(_historyServices.GetAllHistories());
            }
        }

        [RelayCommand]
        void ClearAll() {
            _historyServices.ClearAll();
            ListaHistorial = new ObservableCollection<HistoryModel>(_historyServices.GetAllHistories());
        }

        [RelayCommand]
        void Navegar(HistoryModel? historyModel)
        {
            if(historyModel != null)
            {
                if (_mainWindow != null)
                {
                    var newItem = new ItemModel();
                    newItem.Tab.countitem.DataContext = _mainWindow.lista;
                    newItem.Tab._tabItemVM.Search(historyModel.URL);
                    _mainWindow._viewmodel.Items.Add(newItem);
                    _mainWindow.lista.SelectedItem = newItem;
                    _mainWindow.lista.ScrollIntoView(newItem);
                }
            }
        }

        public void ActualizarVM() {
            ListaHistorial = new ObservableCollection<HistoryModel>(_historyServices.GetAllHistories());
        }
        
    }
}
