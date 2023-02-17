using Wpf.Ui.Common.Interfaces;

namespace WebBrowserMinimalist.Views.Pages
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class MainPage : INavigableView<ViewModels.MainPageViewModel>
    {
        public ViewModels.MainPageViewModel ViewModel
        {
            get;
        }

        public MainPage(ViewModels.MainPageViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }
    }
}