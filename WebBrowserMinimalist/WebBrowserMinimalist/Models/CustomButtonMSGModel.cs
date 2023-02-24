using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace WebBrowserMinimalist.Models
{
    public partial class CustomButtonMSGModel: ObservableObject
    {
        [ObservableProperty]
        string? _Content = "";

        [ObservableProperty]
        Wpf.Ui.Common.ControlAppearance _Appearance = Wpf.Ui.Common.ControlAppearance.Secondary;

        [ObservableProperty]
        bool _IsDefault = false;
        public Action? Action { get; set; }

    }
}
