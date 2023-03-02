using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using WebBrowserMinimalist.Services;

namespace WebBrowserMinimalist.DBA
{
    public class HIstorialDBA
    {
        readonly GlobalService _servicio;
        string? rutaDBH;
        public HIstorialDBA(GlobalService servicio)
        {
            _servicio = servicio;
            rutaDBH = "Data Source=" +_servicio.GetDefaulProfileFolder() + "History;";
        }

        public void ObtenerDatos()
        {
            using (var con = new SqliteConnection(rutaDBH))
            { 
                
            }
        }
    }
}
