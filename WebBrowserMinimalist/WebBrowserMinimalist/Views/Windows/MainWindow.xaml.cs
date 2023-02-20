using CommunityToolkit.Mvvm.ComponentModel;
using JCS;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using WebBrowserMinimalist.Models;
using WebBrowserMinimalist.ViewModels;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace WebBrowserMinimalist.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowViewModel? _viewmodel { get; }

        public MainWindow()
        {       
            InitializeComponent();
            _viewmodel = this.DataContext as MainWindowViewModel;
            Watcher.Watch(this, BackgroundType.Mica, true, true);

            changeThicknes();
            var item = new ItemModel();
            item.Tab.countitem.DataContext = lista;
            _viewmodel.Items.Add(item);

            //SetPageService(pageService);

            //navigationService.SetNavigationControl(RootNavigation);
        }

        #region INavigationWindow methods

        //public Frame GetFrame()
        //    => RootFrame;

        //public INavigation GetNavigation()
        //    => RootNavigation;

        //public bool Navigate(Type pageType)
        //    => RootNavigation.Navigate(pageType);

        //public void SetPageService(IPageService pageService)
        //    => RootNavigation.PageService = pageService;

        public void ShowWindow()
            => Show();

        public void CloseWindow()
            => Close();

        #endregion INavigationWindow methods

        /// <summary>
        /// Raises the closed event.
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Make sure that closing this window will begin the process of closing the application.
            Application.Current.Shutdown();
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

        private void Window_StateChanged(object sender, EventArgs e)
        {
            changeThicknes();
        }

        void changeThicknes() {
            if (this.WindowState == WindowState.Maximized)
            {
                _viewmodel!.MarginWindowState = new Thickness(7);
            }
            else
            {
                _viewmodel!.MarginWindowState = new Thickness(0);
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectItem = lista.SelectedItem as ItemModel;
           if(selectItem != null) {
                if (content.Children.Count > 0)
                {
                    content.Children.Clear();
                }
                content.Children.Add(selectItem.Tab);
                lista.ScrollIntoView(selectItem);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (lista.Items.Count != 1)
            {
                var button = (Wpf.Ui.Controls.Button)e.Source;
                var ctx = (ItemModel)button.DataContext;
                var current = lista.SelectedItem as ItemModel;
                if (current.UID == ctx.UID && lista.Items.Count > 1) {
                    if (lista.SelectedIndex != 0)
                        lista.SelectedIndex -= 1;
                    else if(lista.SelectedIndex != lista.Items.Count - 1)
                        lista.SelectedIndex += 1;
                }
                if (ctx != null) {
                    
                    _viewmodel.DeleteItemCommand.Execute(ctx.UID);
                    
                }
            }    
           
        }
        private void addbutton_Click(object sender, RoutedEventArgs e)
        {
            var newItem = new ItemModel();
            newItem.Tab.countitem.DataContext = lista;
            _viewmodel.Items.Add(newItem);
            lista.SelectedItem = newItem;
            lista.ScrollIntoView(newItem);
            historyList.Visibility = Visibility.Collapsed;
            lista.Visibility = Visibility.Visible;
           
        }

        private void btnDescargasOpen_Click(object sender, RoutedEventArgs e)
        {
            var newItem = new ItemModel();
            newItem.Tab._tabItemVM.Search("edge://downloads/all");
            newItem.Tab.countitem.DataContext = lista;
            _viewmodel.Items.Add(newItem);
            lista.SelectedItem = newItem;
            lista.ScrollIntoView(newItem);
        }

        private void btnHistorialOpen_Click(object sender, RoutedEventArgs e)
        {
            if (historyList.Visibility != Visibility.Visible)
            {
                historyList.Visibility = Visibility.Visible;
                lista.Visibility = Visibility.Collapsed;
            }
            else
            {
                historyList.Visibility = Visibility.Collapsed;
                lista.Visibility = Visibility.Visible;
            }
        }

        private void menuItemCerrarMenosEste_Click(object sender, RoutedEventArgs e)
        {
            if (lista.Items.Count > 1)
            {
                var button = (System.Windows.Controls.MenuItem)e.Source;
                var ctx = (ItemModel)button.DataContext;
                _viewmodel.CerrarTodosMenosEsteCommand.Execute(ctx);
            }
        }

        private void menuItemCerrarTodosAbajo_Click(object sender, RoutedEventArgs e)
        {
            var button = (System.Windows.Controls.MenuItem)e.Source;
            var ctx = (ItemModel)button.DataContext;
            if (lista.SelectedIndex != lista.Items.Count - 1)
                _viewmodel.CerrarTodosHaciaAbajoCommand.Execute(ctx);
        }

    }
}