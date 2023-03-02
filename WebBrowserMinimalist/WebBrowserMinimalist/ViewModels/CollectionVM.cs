using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xaml;
using WebBrowserMinimalist.Models;
using WebBrowserMinimalist.Services;
using WebBrowserMinimalist.Views.Windows;

namespace WebBrowserMinimalist.ViewModels
{
    public partial class CollectionVM : ObservableObject
    {
        readonly CollectionService _collectionService;
        readonly MainWindow? _mainWindow;
        public CollectionVM()
        {
            _collectionService = App.GetService< CollectionService>();
            _mainWindow = App.Current.MainWindow as MainWindow;
            actualizar();

        }

        [ObservableProperty]
        ObservableCollection<CollectionsModel> _collections = new ObservableCollection<CollectionsModel>();

        void actualizar(string? IDContent = null, string? IDCollection = null, TypeOperation? typeOperation = null)
        {
            var lista = _collectionService.GetAll();

            if (IDContent != null && IDCollection != null && typeOperation != null)
            {
                switch (typeOperation)
                {
                    case TypeOperation.Eliminar:
                        var content = Collections.First(x => x.ID == IDCollection).ContentCollection.First(x => x.IDContent == IDContent);
                        Collections.First(x => x.ID == IDCollection).ContentCollection.Remove(content);
                        break;
                    case TypeOperation.Agregar:
                        var newContent = lista.First(x => x.ID == IDCollection).ContentCollection.First(x => x.IDContent == IDContent);
                        Collections.First(x => x.ID == IDCollection).ContentCollection.Add(newContent);
                        break;
                    default:
                        break;
                }
            }
            else if (IDCollection != null && typeOperation != null)
            {
                switch (typeOperation)
                {
                    case TypeOperation.Actualizar:
                        var ColNew = lista.First(x => x.ID == IDCollection);
                        var old = Collections.First(x => x.ID == IDCollection);
                        old.ContentCollection = ColNew.ContentCollection;
                        old.TituloColeccion = ColNew.TituloColeccion;
                        old.Background = ColNew.Background;
                        break;
                    case TypeOperation.Eliminar:
                        Collections.Remove(Collections.First(x => x.ID == IDCollection));
                        break;
                    case TypeOperation.Agregar:
                        var collection = lista.First(x => x.ID == IDCollection);
                        Collections.Add(collection);
                        break;
                }
            }
            else
                this.Collections = new ObservableCollection<CollectionsModel>(lista);
        }



        public async Task<bool> CreateCollection(CollectionsModel collectionsModel)
        { 
            var result = await _collectionService.InsertNewCollection(collectionsModel);
            if (result)
                actualizar(IDContent: null, IDCollection: collectionsModel.ID, typeOperation: TypeOperation.Agregar);
            return result;
        }

        public async Task<bool> CreateContent(ContentColletionModel contentColletionModel)
        { 
            var result = await _collectionService.InsertNewContentCollection(contentColletionModel);
            if (result)
                actualizar(IDContent: contentColletionModel.IDContent, 
                   IDCollection: contentColletionModel.IDCollection, typeOperation: TypeOperation.Agregar);
            return result;
        }

        public async Task<bool> UpdateCollection(CollectionsModel collectionsModel)
        { 
            var result = await _collectionService.UpdateCollection(collectionsModel);
            if (result)
                actualizar(null, collectionsModel.ID, typeOperation: TypeOperation.Actualizar);
            return result;
        }

       public async Task<bool> DeleteAllCollection(string IDCollection)
        {
            var result = await _collectionService.DeleteAllCollection(IDCollection);
            if (result)
                actualizar(IDContent: null,
                    IDCollection: IDCollection, 
                    typeOperation: TypeOperation.Eliminar);
            return result;
        }

        public async Task<bool> DeleteOneContentCollection(string IDContent)
        {
           var result = await _collectionService.DeleteOneContentCollection(IDContent);
            if (result != null)
                actualizar(IDContent, result.IDCollection, TypeOperation.Eliminar);
            return true;
        }

       public void Navegar(ContentColletionModel? content, WebView2 webview)
        {
            if (content != null)
            {
                if (_mainWindow != null)
                {
                    var newItem = new ItemModel();
                    newItem.Tab._tabItemVM.Search(content.URl, webview);
                    _mainWindow._viewmodel.Items.Add(newItem);
                    _mainWindow.lista.SelectedItem = newItem;
                    _mainWindow.lista.ScrollIntoView(newItem);
                }
            }
        }
    }

    enum TypeOperation
    { 
        Actualizar,
        Eliminar,
        Agregar
    }
}
