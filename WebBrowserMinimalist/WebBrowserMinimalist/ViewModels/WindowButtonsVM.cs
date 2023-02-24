using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBrowserMinimalist.Views.Windows;
using Wpf.Ui.Common;

namespace WebBrowserMinimalist.ViewModels
{
    public partial class WindowButtonsVM : ObservableObject
    {
        readonly MainWindow? _mainWindow;
        public WindowButtonsVM()
        {
            _mainWindow = App.Current.MainWindow as MainWindow;
            if (_mainWindow != null)
                _mainWindow.StateChanged += _mainWindow_StateChanged;
            ChangeIcon();
        }

        [ObservableProperty]
        SymbolRegular _maxOrNormalIcon = SymbolRegular.Square16;

        [RelayCommand]
        void Minimized() {
            if (_mainWindow != null)
            {
                _mainWindow.WindowState = System.Windows.WindowState.Minimized;
            }
        }

        [RelayCommand]
        void Close()
        { 
            App.Current.Shutdown();
        }

        [RelayCommand]
        void MaxOrNormal()
        {
            if (_mainWindow != null)
            {
                if (_mainWindow.WindowState != System.Windows.WindowState.Maximized)
                {
                    _mainWindow.WindowState = System.Windows.WindowState.Maximized;
                    MaxOrNormalIcon = SymbolRegular.SquareMultiple24;
                }
                else
                {
                    _mainWindow.WindowState = System.Windows.WindowState.Normal;
                    MaxOrNormalIcon = SymbolRegular.Square16;
                }
            }
        }

        private void _mainWindow_StateChanged(object? sender, EventArgs e)
        {
            ChangeIcon();
        }

        void ChangeIcon()
        {
            if (_mainWindow != null)
            {
                if (_mainWindow.WindowState == System.Windows.WindowState.Maximized)
                {
                    MaxOrNormalIcon = SymbolRegular.SquareMultiple24;
                }
                else
                {
                    MaxOrNormalIcon = SymbolRegular.Square16;
                }
            }
        }
    }
}
