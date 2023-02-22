using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WebBrowserMinimalist.Models
{
    public partial class MSNModel : ObservableObject
    {
        [ObservableProperty]
        string _Title = "Mensaje";

        [ObservableProperty]
        string _Content = "Mensaje";

        [ObservableProperty]
        Visibility _ButtonYesNoVisibility = Visibility.Collapsed;
        [ObservableProperty]
        Visibility _ButtonOkCancelVisibility = Visibility.Collapsed;
        [ObservableProperty]
        Visibility _ButtonOkVisibility = Visibility.Visible;
        [ObservableProperty]
        Visibility _ButtonYesNoCancelVisibility = Visibility.Collapsed;

        public MessageBoxResult MessageBoxResult { get; set; }

    }


}
