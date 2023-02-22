using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WebBrowserMinimalist.Models;
using WebBrowserMinimalist.Views.Controls.Commons;

namespace WebBrowserMinimalist.Services
{
    public class MensajeService
    {
        public MessageBoxResult ShowDialog(string contenido)
        {
            var msgmodel = new MSNModel()
            {
                Content = contenido,
            };
            var msg = new MSG(msgmodel).ShowDialog();
            return msgmodel.MessageBoxResult;
        }

        public MessageBoxResult ShowDialog(string contenido, string titulo)
        {
            var msgmodel = new MSNModel()
            {
                Content = contenido,
                Title = titulo,
            };
            var msg = new MSG(msgmodel).ShowDialog();
            return msgmodel.MessageBoxResult;
        }

        public MessageBoxResult ShowDialog(string contenido, string titulo, MessageBoxButton Buttons)
        {
            var msgmodel = new MSNModel()
            {
                Content = contenido,
                Title = titulo,
                ButtonOkVisibility = Buttons == MessageBoxButton.OK ? Visibility.Visible : Visibility.Collapsed,
                ButtonOkCancelVisibility = Buttons == MessageBoxButton.OKCancel ? Visibility.Visible : Visibility.Collapsed,
                ButtonYesNoCancelVisibility = Buttons == MessageBoxButton.YesNoCancel ? Visibility.Visible : Visibility.Collapsed,
                ButtonYesNoVisibility = Buttons == MessageBoxButton.YesNo ? Visibility.Visible : Visibility.Collapsed,
            };
            var msg = new MSG(msgmodel).ShowDialog();
            return msgmodel.MessageBoxResult;
        }        
    }
}
