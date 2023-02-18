using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WebBrowserMinimalist.ViewModels;
using Wpf.Ui.Appearance;

namespace WebBrowserMinimalist.Views.Windows
{
    /// <summary>
    /// Lógica de interacción para Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {

        readonly SettingsVM? _viewmodel;

        public Settings()
        {
            InitializeComponent();
            Watcher.Watch(this, BackgroundType.Mica, true, true);
            _viewmodel = DataContext as SettingsVM;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            var radio = (RadioButton)e.Source;
            if (_viewmodel != null)
            {
                _viewmodel.SetEngineCommand.Execute(radio.CommandParameter);
            }
        }
    }
}
