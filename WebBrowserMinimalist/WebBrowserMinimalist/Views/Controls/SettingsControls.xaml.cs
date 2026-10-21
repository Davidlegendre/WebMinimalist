using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using WebBrowserMinimalist.ViewModels;
using Wpf.Ui.Controls;

namespace WebBrowserMinimalist.Views.Controls
{
    /// <summary>
    /// Lógica de interacción para SettingsControls.xaml
    /// </summary>
    public partial class SettingsControls : UserControl
    {
        readonly SettingsVM? _viewmodel;
        public SettingsControls()
        {
            InitializeComponent();
            _viewmodel = DataContext as SettingsVM;
        }
              

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            var radio = (ToggleButton)e.Source;
            if (_viewmodel != null)
            {
                _viewmodel.SetEngineCommand.Execute(radio.CommandParameter);
            }
        }

    }
}
