using System.Windows;
using WordCheckerApp.ViewModel;

namespace WordCheckerApp.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
