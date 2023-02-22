using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
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

        void actualizar(string? IDContent = null)
        {
            var lista = _collectionService.GetAll();
            this.Collections = new ObservableCollection<CollectionsModel>(lista);
        }



        public async Task<bool> CreateCollection(CollectionsModel collectionsModel)
        { 
            var result = await _collectionService.InsertNewCollection(collectionsModel);
            if (result)
                actualizar();
            return result;
        }

        public async Task<bool> CreateContent(ContentColletionModel contentColletionModel)
        { 
            var result = await _collectionService.InsertNewContentCollection(contentColletionModel);
            if (result)
                actualizar();
            return result;
        }

        public async Task<bool> UpdateCollection(CollectionsModel collectionsModel)
        { 
            var result = await _collectionService.UpdateCollection(collectionsModel);
            if (result)
                actualizar();
            return result;
        }

       public async Task<bool> DeleteAllCollection(string IDCollection)
        {
            var result = await _collectionService.DeleteAllCollection(IDCollection);
            if (result)
                actualizar();
            return result;
        }

        public async Task<bool> DeleteOneContentCollection(string IDContent)
        {
           var result = await _collectionService.DeleteOneContentCollection(IDContent);
            if (result)
                actualizar(IDContent);
            return result;
        }

       public void Navegar(ContentColletionModel? content)
        {
            if (content != null)
            {
                if (_mainWindow != null)
                {
                    var newItem = new ItemModel();
                    newItem.Tab.countitem.DataContext = _mainWindow.lista;
                    newItem.Tab._tabItemVM.Search(content.URl);
                    _mainWindow._viewmodel.Items.Add(newItem);
                    _mainWindow.lista.SelectedItem = newItem;
                    _mainWindow.lista.ScrollIntoView(newItem);
                }
            }
        }
    }
}
