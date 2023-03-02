using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Xps.Serialization;
using WebBrowserMinimalist.Models;
using WebBrowserMinimalist.Services;

namespace WebBrowserMinimalist.Views.Controls
{
    /// <summary>
    /// Lógica de interacción para Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        readonly GlobalService _globalService;
        public Home()
        {
            InitializeComponent();
            _globalService = App.GetService<GlobalService>();
            _globalService.TimeSystemEvent += _globalService_TimeSystemEvent;
            _globalService.InitTimerHour();
            this.Loaded += Home_Loaded;
            
        }

        private void Home_Loaded(object sender, RoutedEventArgs e)
        {
            Wpf.Ui.Animations.Transitions.ApplyTransition(this, Wpf.Ui.Animations.TransitionType.FadeIn, 500);
            Wpf.Ui.Animations.Transitions.ApplyTransition(Symbol, Wpf.Ui.Animations.TransitionType.SlideRight, 700);
            txtFechaLarga.Text = DateTime.Now.ToLongDateString();
            var hora = DateTime.Now.ToString("hh:mm");
            txthour.Text = hora.Substring(0, 2);
            txtmin.Text = hora.Substring(3);
            txttimeAMPM.Text = (DateTime.Now.Hour > 12 ? TypeHora.PM : TypeHora.AM) == TypeHora.AM ? "AM" : "PM";
            saludar(DateTime.Now.Hour, (DateTime.Now.Hour > 12 ? TypeHora.PM : TypeHora.AM));
        }

        private void _globalService_TimeSystemEvent(object? sender, HomeDataModel e)
        {
            txtFechaLarga.Text = e.Fecha;
            txthour.Text = e.Hora.Substring(0, 2);
            txtmin.Text = e.Hora.Substring(3);
            txttimeAMPM.Text = e.TypeHora == TypeHora.AM ? "AM" : "PM";
            saludar(e.Hora24, e.TypeHora);
        }


        void saludar(int hour, TypeHora typeHora)
        {
            if (typeHora == TypeHora.AM)
            {
                if (hour >= 0 && hour <= 12)
                    txtSaludo.Text = EnumSaludos.HolaBuenosDías;
            }
            else
            {
                if (hour > 12 && hour <= 18)
                    txtSaludo.Text = EnumSaludos.HolaBuenasTardes;
                if (hour > 18 && hour <= 24)
                    txtSaludo.Text = EnumSaludos.HolaBuenasNoches;
            }

            if(hour >= 0 && hour <= 5)
                Symbol.Symbol = Wpf.Ui.Common.SymbolRegular.WeatherPartlyCloudyNight20;
            if (hour > 5 && hour <= 7)
                Symbol.Symbol = Wpf.Ui.Common.SymbolRegular.WeatherSunnyLow48;
            if(hour > 7 && hour <= 10)
                Symbol.Symbol = Wpf.Ui.Common.SymbolRegular.WeatherSunnyHigh48;
            if (hour > 10 && hour <= 14)
                Symbol.Symbol = Wpf.Ui.Common.SymbolRegular.WeatherSunny48;
            if (hour > 14 && hour <= 18)
                Symbol.Symbol = Wpf.Ui.Common.SymbolRegular.WeatherPartlyCloudyDay48;
            if (hour > 18)
                Symbol.Symbol = Wpf.Ui.Common.SymbolRegular.WeatherMoon48;
        }
    }

    static internal class EnumSaludos {
        public static string HolaBuenosDías = "Buenos Días";
        public static string HolaBuenasTardes = "Buenas Tardes";
        public static string HolaBuenasNoches = "Buenas Noches";
    }
}
