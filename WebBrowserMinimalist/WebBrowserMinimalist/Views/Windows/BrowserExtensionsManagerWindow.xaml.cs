using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WebBrowserMinimalist.Services;
using Wpf.Ui.Controls;

namespace WebBrowserMinimalist.Views.Windows
{
    public partial class BrowserExtensionsManagerWindow : UiWindow
    {
        private readonly CoreWebView2Profile _profile;
        private readonly BrowserExtensionService _extensionService;
        private readonly ObservableCollection<BrowserExtensionItem> _extensionItems = new();

        public BrowserExtensionsManagerWindow(
            CoreWebView2Profile profile,
            BrowserExtensionService extensionService)
        {
            InitializeComponent();
            _profile = profile;
            _extensionService = extensionService;
            BrowserExtensionsListBox.ItemsSource = _extensionItems;
            Loaded += async (_, _) => await RefreshBrowserExtensionsAsync();
        }

        private void OpenExtensionImporter_Click(object sender, RoutedEventArgs e)
        {
            AddPaneColumn.Width = new GridLength(300);
            AddExtensionPane.Visibility = Visibility.Visible;
            ExtensionSourceTextBox.Focus();
        }

        private void CloseAddPane_Click(object sender, RoutedEventArgs e)
        {
            AddExtensionPane.Visibility = Visibility.Collapsed;
            AddPaneColumn.Width = new GridLength(0);
            ImportStatusTextBlock.Text = string.Empty;
            ExtensionSourceTextBox.Clear();
        }

        private async void Import_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ExtensionSourceTextBox.Text))
            {
                ImportStatusTextBlock.Text = "Introduce una URL o un ID de Chrome Web Store.";
                return;
            }

            string extensionId;
            try
            {
                extensionId = BrowserExtensionService.ParseExtensionId(ExtensionSourceTextBox.Text);
            }
            catch (ArgumentException exception)
            {
                ImportStatusTextBlock.Text = $"URL o ID no válido: {exception.Message}";
                return;
            }

            SetImportBusy(true);
            ImportStatusTextBlock.Text = "Descargando y validando la extensión…";
            try
            {
                var extension = await _extensionService.InstallFromChromeWebStoreAsync(_profile, extensionId);
                ImportStatusTextBlock.Text = $"Extensión instalada: {extension.Name}.";
                ExtensionSourceTextBox.Clear();
                await RefreshBrowserExtensionsAsync();
            }
            catch (Exception exception)
            {
                ImportStatusTextBlock.Text = $"No se pudo instalar la extensión: {exception.Message}";
            }
            finally
            {
                SetImportBusy(false);
            }
        }

        private void SetImportBusy(bool isBusy)
        {
            ExtensionSourceTextBox.IsEnabled = !isBusy;
            ImportButton.IsEnabled = !isBusy;
            CancelImportButton.IsEnabled = !isBusy;
        }

        private async void ToggleBrowserExtension_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is not BrowserExtensionItem item)
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
            if ((sender as FrameworkElement)?.DataContext is not BrowserExtensionItem item)
                return;

            var result = System.Windows.MessageBox.Show(
                this,
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
            try
            {
                var extensions = await _extensionService.GetInstalledExtensionsAsync(_profile);
                _extensionItems.Clear();
                foreach (var extension in extensions)
                {
                    var iconPath = _extensionService.GetInstalledExtensionIconPath(extension.Id);
                    _extensionItems.Add(new BrowserExtensionItem(extension, LoadExtensionIcon(iconPath)));
                }

                ExtensionsStatusTextBlock.Text = extensions.Count == 0
                    ? "No hay extensiones instaladas. Añade una para empezar."
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

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        private sealed class BrowserExtensionItem
        {
            public BrowserExtensionItem(CoreWebView2BrowserExtension extension, ImageSource? icon)
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
    }
}
