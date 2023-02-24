﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebBrowserMinimalist.Models;
using WebBrowserMinimalist.Services;
using WebBrowserMinimalist.ViewModels;
using WebBrowserMinimalist.Views.Controls.Collections_Windows;
using WebBrowserMinimalist.Views.Windows;

namespace WebBrowserMinimalist.Views.Controls
{
    /// <summary>
    /// Lógica de interacción para CollectionsVMVertical.xaml
    /// </summary>
    public partial class CollectionsVMVertical : UserControl
    {
        readonly CollectionVM? _VM;
        readonly MainWindow? _MainWindow;
        readonly MensajeService _msn;
        public CollectionsVMVertical()
        {
            InitializeComponent();
            _VM = DataContext as CollectionVM;
            _MainWindow = App.Current.MainWindow as MainWindow;
            _msn = App.GetService<MensajeService>();
        }

        private void btnLinkContentColectionItem_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Wpf.Ui.Controls.Button;
            var collection = button.Tag as ContentColletionModel;
            var itemSelected = _MainWindow.lista.SelectedItem as ItemModel;
            if(itemSelected != null)
            {
                _VM.Navegar(collection);
            }
        }

        private async void btnAddContentItem_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Wpf.Ui.Controls.Button;
            var collection = button.Tag as CollectionsModel;
            var itemSelected = _MainWindow.lista.SelectedItem as ItemModel;
            if (itemSelected != null && !string.IsNullOrWhiteSpace(itemSelected.Source))
            {
                var result = await _VM.CreateContent(new ContentColletionModel()
                {
                    IDContent = Guid.NewGuid().ToString(),
                    TituloDocumento = itemSelected.TitleDoc,
                    URl = itemSelected.Source,
                    IDCollection = collection.ID
                });
            }
        }

        private void btnEditCollection_Click(object sender, RoutedEventArgs e)
        {
            var button = (MenuItem)sender;
            var collection = button.Tag as CollectionsModel;
            if (_MainWindow != null && _VM != null)
            {
                var agregarModWin = new AgregarColeccionWindow(_MainWindow, _VM, TypeDataTranfered.Edit);
                agregarModWin.txtTitulo.Text = collection.TituloColeccion;
                agregarModWin.Tag = collection.ID;
                foreach (var card in agregarModWin.listColor.Items) {
                    if (((Wpf.Ui.Controls.CardColor)card).Color.ToString() == collection.Background) {
                        agregarModWin.listColor.SelectedItem = card;
                        agregarModWin.listColor.ScrollIntoView(card);
                        break;
                    }
                }
                agregarModWin.ShowDialog();

            }
        }

        private async void btnDelteCollection_Click(object sender, RoutedEventArgs e)
        {
            var button = (MenuItem)sender;
            var collection = button.Tag as CollectionsModel;
            if(_VM != null)
            {
                if (_msn.ShowDialog("Desea Eliminar la coleccion: " + collection.TituloColeccion + "?", "Eliminando", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var result = await _VM.DeleteAllCollection(collection.ID);
                }
            }
        }

        private void btnAddCollection_Click(object sender, RoutedEventArgs e)
        {
            if (_MainWindow != null && _VM != null)
            {
                var agregarModWin = new AgregarColeccionWindow(_MainWindow, _VM);
                agregarModWin.ShowDialog();
                
            }
        }


        private async void EliminarContent_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as MenuItem;
            var collection = button.Tag as ContentColletionModel;
            if (collection != null)
            {
                await _VM.DeleteOneContentCollection(collection.IDContent);
            }
        }
    }
}
