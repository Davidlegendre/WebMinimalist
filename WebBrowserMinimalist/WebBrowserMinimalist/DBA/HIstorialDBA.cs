using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using WebBrowserMinimalist.Models;
using WebBrowserMinimalist.Services;

namespace WebBrowserMinimalist.DBA
{
    public class HIstorialDBA
    {
        readonly GlobalService _servicio;
        string rutaDBH, connection;
        public HIstorialDBA(GlobalService servicio)
        {
            _servicio = servicio;
            rutaDBH = _servicio.GetDefaulProfileFolder() + "History";
            connection = $"data source={rutaDBH}";
        }

        public async Task<IEnumerable<HistoryModel>> ObtenerDatos()
        {
            //copio porque sino lo hago ne dice que database is locked
            File.Copy(rutaDBH, rutaDBH + ".db", true);
            using (var con = new SQLiteConnection(connection + ".db"))
            {
                var result =  await con.ExecuteReaderAsync("SELECT id, url, title FROM urls");
                List<HistoryModel> list = new List<HistoryModel>();
                while (result.Read())
                {
                    list.Add(new HistoryModel
                    {
                        id = result.GetInt64(0),
                        title = result.GetString(2),
                        url = result.GetString(1)
                    });
                }
                return list.DistinctBy(x=>x.title);
            }
        }
    }
}
