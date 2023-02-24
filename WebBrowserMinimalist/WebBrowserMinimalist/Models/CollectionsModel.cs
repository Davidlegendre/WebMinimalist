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
    /// <summary>
    /// Dapper es capaz de hacer match hasta con una clase partial con observable
    /// </summary>
    public partial class CollectionsModel : ObservableObject
    {
        [ObservableProperty]
        string? _ID;
        [ObservableProperty]
        string? _TituloColeccion;
        [ObservableProperty]
        string? _Background;

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
