using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBrowserMinimalist.Models
{
   public class HomeDataModel
    {
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public int Hora24 { get; set; }
        public TypeHora TypeHora { get; set; }
    }
   public enum TypeHora { 
        PM,
        AM
    }
    
}
