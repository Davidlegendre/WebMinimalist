﻿using CommunityToolkit.Mvvm.ComponentModel;
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
using WebBrowserMinimalist.Services;
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
        readonly OperacionesService _operaciones;
        readonly MensajeService _maensajeService;
        readonly GlobalService _globalService;
        public MainWindow()
        {       
            InitializeComponent();
            _operaciones = App.GetService<OperacionesService>();
            _maensajeService = App.GetService<MensajeService>();
            _globalService = App.GetService<GlobalService>();
            _viewmodel = this.DataContext as MainWindowViewModel;
            changeThicknes();
           
            Watcher.Watch(this, BackgroundType.Mica, true, true);
            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
            //SetPageService(pageService);

            //navigationService.SetNavigationControl(RootNavigation);
        }

        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            _globalService.DisposeTimeHour();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CargaArchivosExternos();
        }

        void CargaArchivosExternos() {
            if (Environment.CommandLine.Split(" ").Length > 1)
            {
                var ruta = Environment.CommandLine.Split('"')[1];

                if (_operaciones.IsArchivoAdmitido(ruta) == true || ruta.StartsWith("http:") || ruta.StartsWith("https:"))
                {

                    var item = new ItemModel();
                    item.Tab._tabItemVM.Search(ruta);
                    item.Tab.countitem.DataContext = lista;
                    _viewmodel.Items.Add(item);
                }
                else
                {
                    var item = new ItemModel();
                    item.Tab.countitem.DataContext = lista;
                    _viewmodel.Items.Add(item);
                }
            }
            else
            {
                var item = new ItemModel();
                item.Tab.countitem.DataContext = lista;
                _viewmodel.Items.Add(item);
            }
            
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
        public void addbutton_Click(object sender, RoutedEventArgs e)
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
                btnAtras.Visibility = Visibility.Visible;
                historyList.Visibility = Visibility.Visible;
                lista.Visibility = Visibility.Collapsed;
                Wpf.Ui.Animations.Transitions.ApplyTransition(historyList, Wpf.Ui.Animations.TransitionType.SlideRight, 400);
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

        private void btnAtras_Click(object sender, RoutedEventArgs e)
        {


            if (lista.Visibility != Visibility.Visible)
            {
                historyList.Visibility = Visibility.Collapsed;
                lista.Visibility = Visibility.Visible;
                btnAtras.Visibility = Visibility.Collapsed;
                
                Wpf.Ui.Animations.Transitions.ApplyTransition(lista, Wpf.Ui.Animations.TransitionType.SlideLeft, 400);
            }
        }

        private void btnCollectionsOpen_Click(object sender, RoutedEventArgs e)
        {
            if (Colecciones.Visibility != Visibility.Visible)
            {
                //btnAtras.Visibility = Visibility.Visible;
                Colecciones.Visibility = Visibility.Visible;
                flyoutPanel.IsOpen = false;
                //lista.Visibility = Visibility.Collapsed;
                Wpf.Ui.Animations.Transitions.ApplyTransition(Colecciones, Wpf.Ui.Animations.TransitionType.FadeInWithSlide, 400);
            }
            else
            {
                Colecciones.Visibility = Visibility.Collapsed;
                flyoutPanel.IsOpen = false;

            }
        }

        private void ProgressRing_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var progressRing = (Wpf.Ui.Controls.ProgressRing)sender;
            if (progressRing.Visibility != Visibility.Visible)
                progressRing.IsIndeterminate = false;
            else
                progressRing.IsIndeterminate = true;
        }

        private void btnCloseCollections_Click(object sender, RoutedEventArgs e)
        {
            if (Colecciones.Visibility == Visibility.Visible)
            {
                Colecciones.Visibility = Visibility.Collapsed;
            }
        }
    }
}