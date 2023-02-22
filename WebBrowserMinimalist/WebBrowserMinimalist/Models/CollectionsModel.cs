using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WebBrowserMinimalist.Models
{
    public partial class CollectionsModel : ObservableObject
    {
        public string ID { get; set; }
        public string? TituloColeccion { get; set; }
        public string? Background { get; set; }

        [ObservableProperty]
        ObservableCollection<ContentColletionModel> _ContentCollection = new ObservableCollection<ContentColletionModel>();
    }

    public class ContentColletionModel {
        public string IDContent { get; set; }
        public string? TituloDocumento { get; set; }
        public string? URl { get; set; }
        public string IDCollection { get; set; }
    }
}
