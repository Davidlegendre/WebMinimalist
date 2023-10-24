using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SQLite;
using WebBrowserMinimalist.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media;
using WebBrowserMinimalist.Models;
using System.Windows.Controls;
using System.Windows;

namespace WebBrowserMinimalist.DBA
{
    public class CollectionsDBA
    {
        readonly GlobalService _globalService;
        string rutaBD;
        string connectionString;
        readonly MensajeService _msn;
        public CollectionsDBA(GlobalService globalService, MensajeService msn)
        {
            _globalService = globalService;
            rutaBD = _globalService.GetFolderAppDomain + "/CollectionsDB.db";
            connectionString = "Data Source=" + rutaBD + ";Version=3;";
            createNewDataBase();
            _msn = msn;
        }

        async void createNewDataBase()
        {
            if (!File.Exists(rutaBD))
            {
                SQLiteConnection.CreateFile(rutaBD);
                using (var con = new SQLiteConnection(connectionString))
                {

                    await con.ExecuteAsync("create table collections (" +
                        "ID text," +
                        "TituloColeccion text," +
                        "Background text )");

                    await con.ExecuteAsync("create table ContentCollection (" +
                        "IDContent text," +
                        "TituloDocumento text," +
                        "URl text," +
                        "IDCollection text" +
                        ")");

                }

            }

        }

        public async Task<bool> InsertNewCollection(CollectionsModel collectionsModel)
        {
            try
            {
                using (var con = new SQLiteConnection(connectionString))
                {
                    collectionsModel.ID = Guid.NewGuid().ToString();
                    await con.ExecuteAsync("insert into collections(ID,TituloColeccion,Background)" +
                        "values('" + collectionsModel.ID + "', '" + collectionsModel.TituloColeccion + "', '" 
                        + collectionsModel.Background + "')");

                }
                return true;
            }
            catch (Exception ex)
            {
                _msn.ShowDialog(ex.Message);
                return false;
            }
        }

        public async Task<bool> InsertNewContentCollection(ContentColletionModel contentColletionModel)
        {
            try
            {
                using (var con = new SQLiteConnection(connectionString))
                {
                    contentColletionModel.IDContent = Guid.NewGuid().ToString();
                   var result = await con.ExecuteAsync("insert into ContentCollection(IDContent,TituloDocumento,URl,IDCollection)" +
                        "values('" + contentColletionModel.IDContent + "', '" +
                        contentColletionModel.TituloDocumento + "','" + contentColletionModel.URl + "', '"
                        + contentColletionModel.IDCollection + "')");
                }

                return true;
            }
            catch (Exception ex)
            {
                _msn.ShowDialog(ex.Message);
                return false;
            }
        }


        public async Task<IEnumerable<CollectionsModel>?> GetAll()
        {
            try
            {
                using (var con = new SQLiteConnection(connectionString))
                {
                    var collections = await con.QueryAsync<CollectionsModel>("select ID, TituloColeccion, Background  from collections");
                    
                    collections.ToList().ForEach(async x => {
                        x.VisibleBookMark = x.Background == "#0FFFFFFF" ? Visibility.Collapsed: Visibility.Visible;
                        var countContent = await con.ExecuteScalarAsync<int>("select count(IDContent) from ContentCollection where IDCollection = '" + x.ID + "'");
                        if (countContent > 0)
                        {
                            var content = await con.QueryAsync<ContentColletionModel>("select IDContent, TituloDocumento,URl," +
                            "IDCollection from ContentCollection where IDCollection = '" + x.ID + "'");
                            content.ToList().ForEach(y => { 
                                x.ContentCollection.Add(y); 
                            });
                        }
                        
                    });
                    return collections;
                }
            }
            catch (Exception ex)
            {
                _msn.ShowDialog(ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<CollectionsModel>?> GetCollections()
        {
            try
            {
                using (var con = new SQLiteConnection(connectionString))
                {
                    return await con.QueryAsync<CollectionsModel>("select ID, TituloColeccion, Background  from collections");
                }
            }
            catch (Exception ex)
            {
                _msn.ShowDialog(ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<ContentColletionModel>?> GetContentsCollections(Guid IDCollection)
        {
            try
            {
                using (var con = new SQLiteConnection(connectionString))
                {
                    return await con.QueryAsync<ContentColletionModel>("select ID, TituloDocumento,URl," +
                        "IDCollection from ContentCollection where IDCollection = '" + IDCollection + "'");
                }
            }
            catch (Exception ex) { _msn.ShowDialog(ex.Message); return null; }            
        }

        public async Task<bool> UpdateCollection(CollectionsModel collectionsModel)
        {
            try
            {
                using (var con = new SQLiteConnection(connectionString))
                {
                    await con.ExecuteAsync("update collections set TituloColeccion = '" 
                        + collectionsModel.TituloColeccion + "', Background = '" + collectionsModel.Background + "'" +
                        " where ID = '" + collectionsModel.ID + "'");
                }
                return true;
            }
            catch (Exception ex) { _msn.ShowDialog(ex.Message); return false; }
        }

        public async Task<bool> DeleteAllCollection(string IDCollection)
        {
            try
            {
                using (var con = new SQLiteConnection(connectionString))
                {
                    await con.ExecuteAsync("delete from ContentCollection where IDCollection = '" + IDCollection + "'");
                    await con.ExecuteAsync("delete from collections where ID = '" + IDCollection + "'");
                }
                return true;
            }
            catch (Exception ex) { _msn.ShowDialog(ex.Message); return false; }
        }

        public async Task<ContentColletionModel?> DeleteOneContentCollection(string IDContent)
        {
            try
            {
                ContentColletionModel contentColletionModel;
                using (var con = new SQLiteConnection(connectionString))
                {
                    contentColletionModel = await con.QueryFirstAsync<ContentColletionModel>("select IDContent, TituloDocumento,URl," +
                        "IDCollection from ContentCollection where IDContent = '" + IDContent + "'");
                    await con.ExecuteAsync("delete from ContentCollection where IDContent = '" + IDContent + "'");
                    
                }
                return contentColletionModel;
            }
            catch (Exception ex) { _msn.ShowDialog(ex.Message); return null; }
        }
    }
}
