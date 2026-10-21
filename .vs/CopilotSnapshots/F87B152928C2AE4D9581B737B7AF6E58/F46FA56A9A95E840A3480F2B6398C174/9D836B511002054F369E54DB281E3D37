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
using WebBrowserMinimalist.Models;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace WebBrowserMinimalist.Views.Controls.Commons
{
    /// <summary>
    /// Lógica de interacción para MSG.xaml
    /// </summary>
    public partial class MSG : INavigationWindow
    {
        MSNModel _msnmodel;
        public MSG(MSNModel mSNModel)
        {
            InitializeComponent();
            this.Closing += MSG_Closing;
            _msnmodel = mSNModel;
            DataContext = _msnmodel;
        }

        private void MSG_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {         
            this.DialogResult = true;
        }

        #region NavigationWindows
        public void CloseWindow()
        {
            throw new NotImplementedException();
        }

        public Frame GetFrame()
        {
            throw new NotImplementedException();
        }

        public INavigation GetNavigation()
        {
            throw new NotImplementedException();
        }

        public bool Navigate(Type pageType)
        {
            throw new NotImplementedException();
        }

        public void SetPageService(IPageService pageService)
        {
            throw new NotImplementedException();
        }

        public void ShowWindow()
        {
            throw new NotImplementedException();
        }


        #endregion

        private void okbtn_Click(object sender, RoutedEventArgs e)
        {
            _msnmodel.MessageBoxResult = MessageBoxResult.OK;
            this.Close();
        }

        private void siBtn_Click(object sender, RoutedEventArgs e)
        {
            _msnmodel.MessageBoxResult = MessageBoxResult.Yes;
            this.Close();
        }

        private void Nobtn_Click(object sender, RoutedEventArgs e)
        {
            _msnmodel.MessageBoxResult = MessageBoxResult.No;
            this.Close();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            _msnmodel.MessageBoxResult = MessageBoxResult.Cancel;
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = (Wpf.Ui.Controls.Button)sender;
            var context = button.DataContext as CustomButtonMSGModel;

           if(context != null) {
                if (context.Action != null)
                    context.Action.Invoke();
            }

            _msnmodel.MessageBoxResult = MessageBoxResult.OK;
            this.Close();
        }
    }
}
