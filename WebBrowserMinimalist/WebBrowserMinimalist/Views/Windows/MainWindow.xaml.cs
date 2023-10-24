using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
    public partial class MainWindow : UiWindow
    {
        public MainWindowViewModel? _viewmodel { get; }
        readonly OperacionesService _operaciones;
        readonly MensajeService _maensajeService;
        readonly GlobalService _globalService;
        public MainWindow()
        {
            InitializeComponent();
            Watcher.Watch(this);
            _operaciones = App.GetService<OperacionesService>();
            _maensajeService = App.GetService<MensajeService>();
            _globalService = App.GetService<GlobalService>();
            _viewmodel = this.DataContext as MainWindowViewModel;

            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
            //SetPageService(pageService);

            //navigationService.SetNavigationControl(RootNavigation);
        }
        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            //if (_maensajeService.ShowDialog("Desea Cerrar el Navegador?", "Saliendo", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                
            //else
            //    e.Cancel = true;
            _globalService.DisposeTimeHour();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CargaArchivosExternos();
            this.ResizeMode = ResizeMode.NoResize;
            WindowState = WindowState.Maximized;
            this.ResizeMode = ResizeMode.CanResize;
            //new pruebas().Show();
        }

        void CargaArchivosExternos()
        {
            //if (Environment.CommandLine.Contains("¬[incognit]"))
            //{
            //    _globalService.IsInPrivateMode = true;
            //}
            var item = new ItemModel();
            //_maensajeService.ShowDialog(Environment.CommandLine);

            try
            {
                //_globalService.IsInPrivateMode = item.Tab.webview.CreationProperties.IsInPrivateModeEnabled.Value;
       
                
                //btnOpenWithIncognit.Visibility = Visibility.Collapsed;
                btnHistorialOpen.Visibility = item.Tab.webview.CreationProperties.IsInPrivateModeEnabled.Value ? Visibility.Collapsed : Visibility.Visible;

                if (Environment.CommandLine.Split(" ").Length > 2)
                {

                    var pre = (Environment.CommandLine.Contains('"') ?
                        Environment.CommandLine.Split(" " + '"')[1]
                        : Environment.CommandLine.Split(" ")[1]);
                    var ruta = pre.EndsWith('"') ? pre.Remove(pre.Length - 1) : pre;
                    item.Tab!.webview.Margin = new Thickness(7, 2, 7, 7);
                    item!.Tab!._tabItemVM!.Search("file:///" + ruta, item.Tab.webview);
                    _viewmodel!.Items!.Add(item);
                }
                else
                {
                    item.Tab!.webview.Margin = new Thickness(7, 2, 7, 7);
                    _viewmodel!.Items!.Add(item);
                }
            }
            catch (Exception ex)
            {
                //_maensajeService.ShowDialog(ex.Message);
                item.Tab!.webview.Margin = new Thickness(7, 2, 7, 7);
                _viewmodel!.Items!.Add(item);
            }

        }

        /// <summary>
        /// Raises the closed event.
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Make sure that closing this window will begin the process of closing the application.
            Application.Current.Shutdown();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {

        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectItem = lista.SelectedItem as ItemModel;
            if (selectItem != null)
            {
                if (content.Children.Count > 0)
                {
                    content.Children.Clear();
                }
                content.Children.Add(selectItem.Tab);
                _viewmodel?.SelectItem(selectItem);
                lista.ScrollIntoView(selectItem);

            }
        }

        public void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (lista.Items.Count != 1)
            {
                var button = (Wpf.Ui.Controls.Button)e.Source;
                ItemModel? ctx = (button.DataContext.GetType() == typeof(ItemModel)) ? (ItemModel)button.DataContext : null;
                var current = lista.SelectedItem as ItemModel;
                if (ctx != null)
                {
                    var index = lista.Items.IndexOf(ctx);
                    if (lista.SelectedIndex == index)
                    {
                        if (lista.Items.Count > 1)
                        {
                            if (lista.SelectedIndex != 0)
                                lista.SelectedIndex -= 1;
                            else if (lista.SelectedIndex != lista.Items.Count - 1)
                                lista.SelectedIndex += 1;
                        }
                    }
                    _viewmodel!.DeleteItemCommand.Execute(ctx!.UID);
                }
                else
                {
                    if (lista.Items.Count > 1)
                    {
                        if (lista.SelectedIndex != 0)
                            lista.SelectedIndex -= 1;
                        else if (lista.SelectedIndex != lista.Items.Count - 1)
                            lista.SelectedIndex += 1;
                    }
                    _viewmodel!.DeleteItemCommand.Execute(current!.UID);

                }
                //tratar de liminar el contexto
                //sino eliminar el seleccionado



            }
            else
            {
                this.Close();
            }
        }


        public void addbutton_Click(object sender, RoutedEventArgs e)
        {
            var newItem = new ItemModel();            
            _viewmodel!.Items!.Add(newItem);
            lista.SelectedItem = newItem;
            lista.ScrollIntoView(newItem);
            //historyList.Visibility = Visibility.Collapsed;
            lista.Visibility = Visibility.Visible;

        }

        private void btnDescargasOpen_Click(object sender, RoutedEventArgs e)
        {
            var newItem = new ItemModel();
            newItem.Tab!._tabItemVM!.Search("edge://downloads/all", newItem.Tab.webview);
            _viewmodel!.Items!.Add(newItem);
            lista.SelectedItem = newItem;
            lista.ScrollIntoView(newItem);
        }

        private void btnHistorialOpen_Click(object sender, RoutedEventArgs e)
        {
            //if (historyList.Visibility != Visibility.Visible)
            //{
            //    btnAtras.Visibility = Visibility.Visible;
            //    historyList.Visibility = Visibility.Visible;
            //    lista.Visibility = Visibility.Collapsed;
            //    Wpf.Ui.Animations.Transitions.ApplyTransition(historyList, Wpf.Ui.Animations.TransitionType.SlideRight, 400);
            //}
        }

        private void menuItemCerrarMenosEste_Click(object sender, RoutedEventArgs e)
        {
            if (lista.Items.Count > 1)
            {
                var button = (System.Windows.Controls.MenuItem)e.Source;
                var ctx = (ItemModel)button.DataContext;
                _viewmodel!.CerrarTodosMenosEsteCommand.Execute(ctx);
            }
        }

        private void menuItemCerrarTodosAbajo_Click(object sender, RoutedEventArgs e)
        {
            var button = (System.Windows.Controls.MenuItem)e.Source;
            var ctx = (ItemModel)button.DataContext;
            if (lista.SelectedIndex != lista.Items.Count - 1)
                _viewmodel!.CerrarTodosHaciaAbajoCommand.Execute(ctx);
        }

        private void btnAtras_Click(object sender, RoutedEventArgs e)
        {
            if (lista.Visibility != Visibility.Visible)
            {
                //historyList.Visibility = Visibility.Collapsed;
                lista.Visibility = Visibility.Visible;
                //btnAtras.Visibility = Visibility.Collapsed;
                Wpf.Ui.Animations.Transitions.ApplyTransition(lista, Wpf.Ui.Animations.TransitionType.SlideLeft, 400);
            }
        }

        private void btnCollectionsOpen_Click(object sender, RoutedEventArgs e)
        {
            if (Colecciones.Visibility != Visibility.Visible)
            {
                //btnAtras.Visibility = Visibility.Visible;
                Colecciones.Visibility = Visibility.Visible;
                menutabs.Visibility = Visibility.Collapsed;
                //lista.Visibility = Visibility.Collapsed;
                Wpf.Ui.Animations.Transitions.ApplyTransition(Colecciones, Wpf.Ui.Animations.TransitionType.FadeIn, 300);
            }
            else
            {
                Colecciones.Visibility = Visibility.Collapsed;
                
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
                menutabs.Visibility = Visibility.Visible;
            }
        }

        private void btnOpenWithIncognit_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(AppDomain.CurrentDomain.BaseDirectory + Application.ResourceAssembly.GetName().Name!.ToString() + ".exe", "¬[incognit]");
        }

        private void btnclosemenu_Click(object sender, RoutedEventArgs e)
        {
            menutabs.Visibility = Visibility.Collapsed;
        }
        private void card_MouseEnter(object sender, MouseEventArgs e)
        {
            var grid = (Grid)sender;
            var btn = (Wpf.Ui.Controls.Button)grid.Tag;
            var context = btn.DataContext as ItemModel;
            if (btn != null && context != null)
            {
                btn.Opacity = 1;
            }
        }

        private void card_MouseLeave(object sender, MouseEventArgs e)
        {
            var grid = (Grid)sender;
            var btn = (Wpf.Ui.Controls.Button)grid.Tag;
            var context = btn.DataContext as ItemModel;
            if (btn != null && context != null)
            {
                btn.Opacity = 0;
            }
        }
    }
}