using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Threading.Tasks;
using WebBrowserMinimalist.DBA;
using WebBrowserMinimalist.Models;

namespace WebBrowserMinimalist.Services
{
    public class CollectionService
    {
        List<CollectionsModel> _collectionsModels= new List<CollectionsModel>();

        readonly CollectionsDBA collectionsDBA;

        public CollectionService(CollectionsDBA collectionsDBA) {
            this.collectionsDBA = collectionsDBA;
        }

        async void actualizar()
        {
            var collections = await collectionsDBA.GetAll();
            if(collections != null)
                _collectionsModels = collections.ToList();
        }

        public async Task<bool> InsertNewCollection(CollectionsModel collection)
        { 
           return  await collectionsDBA.InsertNewCollection(collection);
        }

        public async Task<bool> InsertNewContentCollection(ContentColletionModel contentColletionModel)
        { 
            return await collectionsDBA.InsertNewContentCollection(contentColletionModel);
        }

        public int Count() {
            return _collectionsModels.Count;
        }

        public List<CollectionsModel> GetAll()
        {
            actualizar();
            return _collectionsModels;
        }

        public List<CollectionsModel>? FindByTitleCollection(string Title)
        {
            return _collectionsModels.Where(x => x.TituloColeccion!.ToLower().Contains(Title.ToLower())).ToList();
        }

        public async Task<bool> UpdateCollection(CollectionsModel collections)
        { 
            return await collectionsDBA.UpdateCollection(collections);
        }

        public Task<bool> DeleteAllCollection(string IDCollection)
        { 
            return collectionsDBA.DeleteAllCollection(IDCollection);
        }

        public async Task<bool> DeleteOneContentCollection(string IDContent)
        { 
           return await collectionsDBA.DeleteOneContentCollection(IDContent);
        }
    }
}
