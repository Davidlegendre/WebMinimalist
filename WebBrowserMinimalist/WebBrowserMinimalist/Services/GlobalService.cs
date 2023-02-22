using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBrowserMinimalist.Services
{
    public class GlobalService
    {
        string? ProfileFolderDefault = "";
        public string GetFolderAppDomain => AppDomain.CurrentDomain.BaseDirectory;

        public void SetDefaultProfileFolder(string FolderURl) {
            ProfileFolderDefault = FolderURl;
        }

        public string? GetDefaulProfileFolder() {
            return ProfileFolderDefault;
        }
    }
}
