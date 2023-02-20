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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebBrowserMinimalist.Models;
using WebBrowserMinimalist.ViewModels;

namespace WebBrowserMinimalist.Views.Controls
{
    /// <summary>
    /// Lógica de interacción para HistoryList.xaml
    /// </summary>
    public partial class HistoryList : UserControl
    {
        readonly HistoryListVM? historyListVM;
        public HistoryList()
        {
            InitializeComponent();
            historyListVM = DataContext as HistoryListVM;
        }

        public void Actualizar() {
           if(historyListVM != null)
            {
                historyListVM.ActualizarVM();
            }
        }

        private void historyBtn_Click(object sender, RoutedEventArgs e)
        {
            if(historyListVM != null)
            {
                var btn = (Wpf.Ui.Controls.Button)sender;
                var item = (HistoryModel)btn.DataContext;
                if(item != null)
                {
                    historyListVM.NavegarCommand.Execute(item);
                }
            }
        }

        private void btnClearItem_Click(object sender, RoutedEventArgs e)
        {
            if (historyListVM != null)
            {
                var btn = (Wpf.Ui.Controls.Button)sender;
                var item = (HistoryModel)btn.DataContext;
                if (item != null)
                {
                    historyListVM.ClearHistorialCommand.Execute(item);
                }
            }
        }
    }
}
