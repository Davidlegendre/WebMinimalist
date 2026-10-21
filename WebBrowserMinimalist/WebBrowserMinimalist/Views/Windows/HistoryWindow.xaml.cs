using System.Windows;
using Wpf.Ui.Controls;

namespace WebBrowserMinimalist.Views.Windows
{
    public partial class HistoryWindow : UiWindow
    {
        public HistoryWindow()
        {
            InitializeComponent();
        }

        public void Actualizar()
        {
            historyList.Actualizar();
        }
    }
}
