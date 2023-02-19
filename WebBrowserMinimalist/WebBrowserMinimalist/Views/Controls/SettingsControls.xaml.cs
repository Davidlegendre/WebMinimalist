using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebBrowserMinimalist.ViewModels;
using Wpf.Ui.Controls;
using Wpf.Ui.Mvvm.Interfaces;

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
