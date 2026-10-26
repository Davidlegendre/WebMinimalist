using Microsoft.Web.WebView2.Core;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        private CoreWebView2Profile? _browserProfile;

        public HistoryList()
        {
            InitializeComponent();
            historyListVM = DataContext as HistoryListVM;
        }

        public void SetBrowserProfile(CoreWebView2Profile profile)
        {
            _browserProfile = profile;
        }

        public void Actualizar()
        {
            if (historyListVM != null)
            {
                historyListVM.ActualizarVM();
            }
        }

        private async void ClearHistory_Click(object sender, RoutedEventArgs e)
        {
            if (historyListVM is null || _browserProfile is null)
            {
                ClearStatusTextBlock.Text = "El navegador aún no está listo para borrar su historial.";
                return;
            }

            var selectedRange = (HistoryRangeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            var (startUtc, endUtc) = GetSelectedRangeUtc(HistoryRangeComboBox.SelectedIndex);
            var clearSiteData = ClearSiteDataCheckBox.IsChecked == true;
            var confirmation = clearSiteData
                ? $"¿Borrar el historial de la aplicación y WebView2 ({selectedRange}) e incluir cookies y datos de sitios?"
                : $"¿Borrar el historial de la aplicación y WebView2 ({selectedRange})?";

            if (MessageBox.Show(
                    Window.GetWindow(this),
                    confirmation,
                    "Borrar historial",
                    MessageBoxButton.YesNo,
                    clearSiteData ? MessageBoxImage.Warning : MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            btnClearAll.IsEnabled = false;
            ClearStatusTextBlock.Text = "Borrando…";
            try
            {
                await historyListVM.ClearHistoryAsync(
                    _browserProfile, startUtc, endUtc, clearSiteData);
                ClearStatusTextBlock.Text = "Historial eliminado.";
            }
            catch (Exception exception)
            {
                ClearStatusTextBlock.Text = exception.Message;
            }
            finally
            {
                btnClearAll.IsEnabled = true;
            }
        }

        private static (DateTime? StartUtc, DateTime? EndUtc) GetSelectedRangeUtc(int selectedIndex)
        {
            if (selectedIndex == 5)
                return (null, null);

            var endUtc = DateTime.UtcNow;
            var startUtc = selectedIndex switch
            {
                0 => DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Local).ToUniversalTime(),
                1 => endUtc.AddHours(-1),
                2 => endUtc.AddDays(-1),
                3 => endUtc.AddDays(-7),
                4 => endUtc.AddDays(-28),
                _ => throw new InvalidOperationException("Selecciona un periodo válido.")
            };

            return (startUtc, endUtc);
        }

        private void historyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (historyListVM != null)
            {
                var btn = (Wpf.Ui.Controls.Button)sender;
                var item = (HistoryModel)btn.DataContext;
                if (item != null)
                {
                    historyListVM.NavegarCommand.Execute(item);
                    Window.GetWindow(this)?.Close();
                }
            }

        }

        //private void btnClearItem_Click(object sender, RoutedEventArgs e)
        //{
        //    if (historyListVM != null)
        //    {
        //        var btn = (Wpf.Ui.Controls.Button)sender;
        //        var item = (HistoryModel)btn.DataContext;
        //        if (item != null)
        //        {
        //            historyListVM.ClearHistorialCommand.Execute(item);
        //        }
        //    }
        //}
    }
}
