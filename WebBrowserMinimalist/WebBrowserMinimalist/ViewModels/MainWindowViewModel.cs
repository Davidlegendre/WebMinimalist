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
        ObservableCollection<TabItemModel>? _tabItems = null;


        [ObservableProperty]
        ObservableCollection<ItemModel>? _items = new ObservableCollection<ItemModel>();

       
        [RelayCommand]
        void DeleteItem(Guid id) {
            var item = _items.FirstOrDefault(x => x.UID == id);
            item.Tab.webview.Dispose();
            _items.Remove(item);
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
