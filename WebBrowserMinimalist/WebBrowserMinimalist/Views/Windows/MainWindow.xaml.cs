using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using WebBrowserMinimalist.Models;
using WebBrowserMinimalist.Services;
using WebBrowserMinimalist.ViewModels;
using WebBrowserMinimalist.Views.Controls.Collections_Windows;
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
        private readonly ObservableCollection<BrowserExtensionPanelItem> _browserExtensionItems = new();
        private CoreWebView2Profile? _browserExtensionProfile;
        public MainWindow()
        {
            InitializeComponent();
            BrowserExtensionsListBox.ItemsSource = _browserExtensionItems;
            var windowChrome = WindowChrome.GetWindowChrome(this);
            if (windowChrome is not null)
                windowChrome.ResizeBorderThickness = new Thickness(8);

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
            foreach (var item in _viewmodel?.Items?.ToList() ?? Enumerable.Empty<ItemModel>())
                item.Dispose();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _ = App.GetService<HistoryServices>()?.GetAllHistoriesAsync();
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
                    _viewmodel!.Items!.Add(item);
                }
            }
            catch (Exception ex)
            {
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
                SetExtensionButtonsEnabled(BrowserExtensionsPanel.Visibility != Visibility.Visible);
            }
        }

        public async Task ShowBrowserExtensionsPanelAsync(CoreWebView2Profile profile)
        {
            _browserExtensionProfile = profile;
            BrowserExtensionsPanel.Visibility = Visibility.Visible;
            SetExtensionButtonsEnabled(false);
            await RefreshBrowserExtensionsAsync();
        }

        private void CloseBrowserExtensionsPanel_Click(object sender, RoutedEventArgs e)
        {
            BrowserExtensionsPanel.Visibility = Visibility.Collapsed;
            SetExtensionButtonsEnabled(true);
        }

        private void SetExtensionButtonsEnabled(bool enabled)
        {
            foreach (var item in _viewmodel?.Items?.ToList() ?? Enumerable.Empty<ItemModel>())
                item.Tab?.SetExtensionsButtonEnabled(enabled);
        }

        private async void OpenExtensionImporter_Click(object sender, RoutedEventArgs e)
        {
            var extensionService = App.GetService<BrowserExtensionService>();
            if (_browserExtensionProfile is null || extensionService is null)
            {
                ExtensionsStatusTextBlock.Text = "El navegador todavía se está inicializando.";
                return;
            }

            var importer = new BrowserExtensionsWindow(_browserExtensionProfile, extensionService)
            {
                Owner = this
            };
            importer.ShowDialog();
            await RefreshBrowserExtensionsAsync();
        }

        private async void ToggleBrowserExtension_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is not BrowserExtensionPanelItem item)
                return;

            try
            {
                ExtensionsStatusTextBlock.Text = "Actualizando extensión…";
                await item.Extension.EnableAsync(!item.Extension.IsEnabled);
                await RefreshBrowserExtensionsAsync();
            }
            catch (Exception exception)
            {
                ExtensionsStatusTextBlock.Text = $"No se pudo cambiar el estado: {exception.Message}";
            }
        }

        private async void RemoveBrowserExtension_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is not BrowserExtensionPanelItem item)
                return;

            var result = System.Windows.MessageBox.Show(
                $"¿Quieres quitar la extensión «{item.Name}»?",
                "Quitar extensión",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                ExtensionsStatusTextBlock.Text = "Quitando extensión…";
                await item.Extension.RemoveAsync();
                await RefreshBrowserExtensionsAsync();
            }
            catch (Exception exception)
            {
                ExtensionsStatusTextBlock.Text = $"No se pudo quitar la extensión: {exception.Message}";
            }
        }

        private async Task RefreshBrowserExtensionsAsync()
        {
            var extensionService = App.GetService<BrowserExtensionService>();
            if (_browserExtensionProfile is null || extensionService is null)
            {
                ExtensionsStatusTextBlock.Text = "El navegador todavía se está inicializando.";
                return;
            }

            try
            {
                var extensions = await extensionService.GetInstalledExtensionsAsync(_browserExtensionProfile);
                _browserExtensionItems.Clear();
                foreach (var extension in extensions)
                {
                    var iconPath = extensionService.GetInstalledExtensionIconPath(extension.Id);
                    _browserExtensionItems.Add(new BrowserExtensionPanelItem(extension, LoadExtensionIcon(iconPath)));
                }

                ExtensionsStatusTextBlock.Text = extensions.Count == 0
                    ? "No hay extensiones instaladas. Usa «Importar» para añadir una."
                    : $"{extensions.Count} extensión(es) instalada(s).";
            }
            catch (Exception exception)
            {
                ExtensionsStatusTextBlock.Text = $"No se pudo leer la lista: {exception.Message}";
            }
        }

        private static ImageSource? LoadExtensionIcon(string? iconPath)
        {
            if (string.IsNullOrWhiteSpace(iconPath) || !File.Exists(iconPath))
                return null;

            try
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(iconPath, UriKind.Absolute);
                image.EndInit();
                image.Freeze();
                return image;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private sealed class BrowserExtensionPanelItem
        {
            public BrowserExtensionPanelItem(CoreWebView2BrowserExtension extension, ImageSource? icon)
            {
                Extension = extension;
                Icon = icon;
            }

            public CoreWebView2BrowserExtension Extension { get; }
            public string Name => Extension.Name;
            public string StateText => Extension.IsEnabled ? "Activada" : "Desactivada";
            public string ToggleText => Extension.IsEnabled ? "Desactivar" : "Activar";
            public ImageSource? Icon { get; }
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
            var historyWindow = new HistoryWindow { Owner = this };
            historyWindow.Actualizar();
            historyWindow.ShowDialog();
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

        private void btnrigth_Click(object sender, RoutedEventArgs e)
        {
            var objeto = VisualTreeHelper.GetChild(lista, 0);
            var _scrollviewer = VisualTreeHelper.GetChild(objeto, 0) as ScrollViewer;
            if (_scrollviewer != null)
            {
                _scrollviewer.ScrollToHorizontalOffset(_scrollviewer.HorizontalOffset + 70);
            }
        }

        private void btnleft_Click(object sender, RoutedEventArgs e)
        {
            var objeto = VisualTreeHelper.GetChild(lista, 0);
            var _scrollviewer = VisualTreeHelper.GetChild(objeto, 0) as ScrollViewer;
            if (_scrollviewer != null)
            {
                _scrollviewer.ScrollToHorizontalOffset(_scrollviewer.HorizontalOffset - 70);
            }
        }

        private void lista_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var objeto = VisualTreeHelper.GetChild(lista, 0);
            var _scrollviewer = VisualTreeHelper.GetChild(objeto, 0) as ScrollViewer;
            var oj = _scrollviewer?.Content as ItemsPresenter;
            if (oj != null)
                if (oj.ActualWidth > lista.ActualWidth)
                {
                    if (btnleft.Visibility != Visibility.Visible)
                    {
                        btnleft.Visibility = Visibility.Visible;
                        btnrigth.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    if (btnleft.Visibility != Visibility.Collapsed)
                    {
                        btnleft.Visibility = Visibility.Collapsed;
                        btnrigth.Visibility = Visibility.Collapsed;
                    }
                }
        }

        private void btnAddCollection_Click(object sender, RoutedEventArgs e)
        {
            var _vmcollection = collections.DataContext as CollectionVM;
            if (_vmcollection != null)
            {
                var agregarModWin = new AgregarColeccionWindow(this, _vmcollection);
                agregarModWin.ShowDialog();

            }
        }

    }
}
