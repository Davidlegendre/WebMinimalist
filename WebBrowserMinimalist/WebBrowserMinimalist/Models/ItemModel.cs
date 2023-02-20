using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using WebBrowserMinimalist.Services;
using WebBrowserMinimalist.Views.Controls;
using Wpf.Ui.Common;

namespace WebBrowserMinimalist.Models
{
    public partial class ItemModel : ObservableObject
    {
        public ItemModel() {
            UID = Guid.NewGuid();
            _tabitem = new TabItem(this);
        }

        TabItem? _tabitem; 

        public Guid UID { get; }

        public TabItem? Tab { get => _tabitem; }

        [ObservableProperty]
        string? _titleDoc = "";

        [ObservableProperty]
        BitmapImage? _IMg = new BitmapImage();

        [ObservableProperty]
        Visibility _SoundVisibility = Visibility.Collapsed;

        [ObservableProperty]
        SymbolRegular _ShieldIcon = SymbolRegular.Shield24;

        [ObservableProperty]
        string? _source = "";

        [ObservableProperty]
        Visibility _refreshvisibility = Visibility.Visible;

        [ObservableProperty]
        Visibility _progressVisibility = Visibility.Collapsed;
    }
}
