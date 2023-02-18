﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBrowserMinimalist.Models;
using WebBrowserMinimalist.Services;

namespace WebBrowserMinimalist.ViewModels
{
   public partial class SettingsVM : ObservableObject
    {
        private readonly OperacionesService _operacionesService;
        public SettingsVM() {
            _operacionesService = App.GetService<OperacionesService>();
            Settings.Add(new SettingModel()
            {
                typeSearchEngine = Services.TypeSearchEngine.Google,
                Name = "Google",
                Image = new Uri("https://img.icons8.com/fluency/48/null/google-logo.png", UriKind.RelativeOrAbsolute),
                IsSelected = _operacionesService.GetEngine() == TypeSearchEngine.Google
            });
            Settings.Add(new SettingModel()
            {
                typeSearchEngine = Services.TypeSearchEngine.Bing,
                Name = "Bing",
                Image = new Uri("https://img.icons8.com/fluency/48/null/bing--v1.png", UriKind.RelativeOrAbsolute),
                IsSelected = _operacionesService.GetEngine() == TypeSearchEngine.Bing
            });
            Settings.Add(new SettingModel()
            {
                typeSearchEngine = Services.TypeSearchEngine.DockDockGo,
                Name = "Duck Duck Go",
                Image = new Uri("https://img.icons8.com/fluency/48/null/duckduckgo.png", UriKind.RelativeOrAbsolute),
                IsSelected = _operacionesService.GetEngine() == TypeSearchEngine.DockDockGo
            });
        } 

        [ObservableProperty]
        ObservableCollection<SettingModel> _settings = new ObservableCollection<SettingModel>();


        [RelayCommand]
        void SetEngine(SettingModel model) {
            _operacionesService.SetEngine(model.typeSearchEngine);
        }

    }
}
