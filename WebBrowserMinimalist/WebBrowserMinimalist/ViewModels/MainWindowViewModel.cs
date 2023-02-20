using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using WebBrowserMinimalist.Models;
using WebBrowserMinimalist.Views.Windows;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace WebBrowserMinimalist.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private bool _isInitialized = false;

        static Uri _DefaultUriImg => new Uri("/Assets/applicationIcon-256.png", UriKind.RelativeOrAbsolute);

        public bool IsFullScreen { get; set; } = false;

        [ObservableProperty]
        private string _applicationTitle = String.Empty;

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
        private Thickness _marginWindowState = new Thickness(0);

        [ObservableProperty]
        ObservableCollection<ItemModel>? _items = new ObservableCollection<ItemModel>();

       
        [RelayCommand]
        void DeleteItem(Guid id) {
            var item = _items.FirstOrDefault(x => x.UID == id);
            item.Tab.webview.Dispose();
            _items.Remove(item);
        }

        [RelayCommand]
        void CerrarTodosMenosEste(ItemModel? item) {
            if (item != null)
            {
                ItemModel[]? items2 = new ItemModel[_items.Count];
                 _items.CopyTo(items2, 0);
                for (int i = 0; i < items2.Length; i++)
                {
                    if (items2[i].UID != item.UID)
                    {
                        items2[i].Tab.webview.Dispose();
                        items2[i] = null;
                    }
                }
                Items = new ObservableCollection<ItemModel>(items2.Where(x => x != null));
            }
        }

        [RelayCommand]
        void CerrarTodosHaciaAbajo(ItemModel? item)
        {
            if (item != null)
            {
                ItemModel[]? items2 = new ItemModel[_items.Count];
                _items.CopyTo(items2, 0);
                var index = Array.IndexOf(items2, item);
                for (int i = index; i < items2.Length; i++)
                {
                    if (items2[i].UID != item.UID)
                    {
                        items2[i].Tab.webview.Dispose();
                        items2[i] = null;
                    }
                }
                Items = new ObservableCollection<ItemModel>(items2.Where(x => x != null));
            }
        }

        public MainWindowViewModel()
        {
            if (!_isInitialized)
                InitializeViewModel(); 
           
        }

        private void InitializeViewModel()
        {
            ApplicationTitle = "UI - WebBrowserMinimalist";
           
            _isInitialized = true;
           
        }


    }
}
