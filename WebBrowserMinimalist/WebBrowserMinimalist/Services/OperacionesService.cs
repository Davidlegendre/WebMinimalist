using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WebBrowserMinimalist.Services
{
   public class OperacionesService
    {

        static TypeSearchEngine _engine = TypeSearchEngine.Bing;

       static Dictionary<TypeSearchEngine, string> SettingEngines= new Dictionary<TypeSearchEngine, string>() {
           { TypeSearchEngine.Google, "https://www.google.com/search?q="},
           { TypeSearchEngine.Bing, "https://www.bing.com/search?q="},
           { TypeSearchEngine.DockDockGo, "https://duckduckgo.com/?q="}
       };


        public void SetEngine(TypeSearchEngine engine) {
            _engine = engine;
        }

        public TypeSearchEngine GetEngine()
        {
            return _engine;
        }

        public string? GetURlEngine() {
            return SettingEngines.GetValueOrDefault(_engine);
        }

       public BitmapImage GetBitmap(Stream stream)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = stream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }



    }

   public enum TypeSearchEngine { 
        Google,
        Bing,
        DockDockGo
    }
}
