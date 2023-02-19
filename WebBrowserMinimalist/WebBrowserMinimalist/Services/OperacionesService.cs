using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WebBrowserMinimalist.Services
{
   public class OperacionesService
    {

        static TypeSearchEngine _engine = TypeSearchEngine.Bing;

       static Dictionary<TypeSearchEngine, Valores> SettingEngines= new Dictionary<TypeSearchEngine, Valores>() {
           { TypeSearchEngine.Google, new Valores(){ URL = "https://www.google.com/search?q=", Value = 0 } },
           { TypeSearchEngine.Bing, new Valores(){ URL =  "https://www.bing.com/search?q=", Value = 1} },
           { TypeSearchEngine.DockDockGo, new Valores() { URL = "https://duckduckgo.com/?q=", Value = 2 }}
       };


        public OperacionesService() {
            var value = WebBrowserMinimalist.Properties.Configurations.Default.MotorBusqueda;
            _engine = SettingEngines.FirstOrDefault(x => x.Value.Value == value).Key;
            //var value = ConfigurationManager.AppSettings.Get("MotorSelect");
            //_engine = SettingEngines.FirstOrDefault(x => x.Value.Value == Convert.ToInt32(value)).Key;
        }

        public void SetEngine(TypeSearchEngine engine)
        {
            _engine = engine;
            //ConfigurationManager.AppSettings.Set("MotorSelect", Convert.ToInt16(engine).ToString());
            WebBrowserMinimalist.Properties.Configurations.Default.MotorBusqueda = ((int)engine);
           
            WebBrowserMinimalist.Properties.Configurations.Default.Save();
            WebBrowserMinimalist.Properties.Configurations.Default.Upgrade();
        }

        public TypeSearchEngine GetEngine()
        {
            return _engine;
        }

        public string? GetURlEngine() {


            return SettingEngines.GetValueOrDefault(_engine)?.URL;
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

    internal class Valores
    { 
        public string URL { get; set; }
        public int Value { get; set; }
    }

   public enum TypeSearchEngine { 
        Google,
        Bing,
        DockDockGo
    }
}
