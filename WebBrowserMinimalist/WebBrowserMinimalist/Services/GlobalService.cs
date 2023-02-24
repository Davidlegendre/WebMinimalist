using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Runtime.InteropServices.ObjectiveC;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebBrowserMinimalist.Models;

namespace WebBrowserMinimalist.Services
{
    public class GlobalService
    {
        string? ProfileFolderDefault = "";
        public string GetFolderAppDomain => AppDomain.CurrentDomain.BaseDirectory;

        Thread? TimeHour;
        bool IsOnline = true;

        public void SetDefaultProfileFolder(string FolderURl) {
            ProfileFolderDefault = FolderURl;
        }

        public string? GetDefaulProfileFolder() {
            return ProfileFolderDefault;
        }

        public void InitTimerHour() {
            if(TimeHour== null)
            {
                IsOnline = true;
                TimeHour = new Thread(() =>
                {
                    while (IsOnline)
                    {
                        if (TimeSystemEvent != null)
                        {
                            App.Current.Dispatcher.Invoke(() =>
                            {
                                TimeSystemEvent.Invoke(this, new HomeDataModel()
                                {
                                    Fecha = DateTime.Now.ToLongDateString(),
                                    Hora = DateTime.Now.ToString("hh:mm"),
                                    Hora24 = DateTime.Now.Hour,
                                    TypeHora = DateTime.Now.Hour > 12 ? TypeHora.PM : TypeHora.AM
                                });
                            });
                            Thread.Sleep(1000);
                        }
                    }
                });
                TimeHour.IsBackground = true;
                TimeHour.Start();
            }
        }

        public void DisposeTimeHour() { 
            if(TimeHour != null )
            {
                IsOnline = false;
                TimeHour = null;
            }
        }

        public event EventHandler<HomeDataModel>? TimeSystemEvent;


    }
}
