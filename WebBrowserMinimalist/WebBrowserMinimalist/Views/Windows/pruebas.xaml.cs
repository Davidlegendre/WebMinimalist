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
using WebBrowserMinimalist.Services;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace WebBrowserMinimalist.Views.Windows
{
    /// <summary>
    /// Lógica de interacción para pruebas.xaml
    /// </summary>
    public partial class pruebas : INavigationWindow
    {
        //readonly GlobalService _service;
        public pruebas()
        {
            InitializeComponent();
            //_service = App.GetService<GlobalService>();
            //_service.descargaEvent += _service_descargaEvent;
            this.Loaded += Pruebas_Loaded;
        }

        private void Pruebas_Loaded(object sender, RoutedEventArgs e)
        {
            Wpf.Ui.Animations.Transitions.ApplyTransition(this, Wpf.Ui.Animations.TransitionType.FadeInWithSlide, 1000);
        }

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

        //private void _service_descargaEvent(object? sender, List<descargaModel> e)
        //{
        //    descargas.ItemsSource = null;
        //    descargas.ItemsSource = e;
        //}
    }
}
