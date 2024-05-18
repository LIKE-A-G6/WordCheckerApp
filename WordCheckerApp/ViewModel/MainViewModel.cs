using System.Windows.Input;
using System.Windows.Controls;
using WordCheckerApp.View;

namespace WordCheckerApp.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public ICommand ShowTemplateSettingsCommand { get; }
        public ICommand ShowDocumentCheckCommand { get; }

        public MainViewModel()
        {
            ShowTemplateSettingsCommand = new RelayCommand(param => ShowTemplateSettings());
            ShowDocumentCheckCommand = new RelayCommand(param => ShowDocumentCheck());
        }

        private void ShowTemplateSettings()
        {
            CurrentView = new TemplateSettingsControl { DataContext = new TemplateSettingsViewModel() };
        }

        private void ShowDocumentCheck()
        {
            CurrentView = new DocumentCheckControl { DataContext = new DocumentCheckViewModel() };
        }

        private UserControl currentView;
        public UserControl CurrentView
        {
            get { return currentView; }
            set { currentView = value; OnPropertyChanged(nameof(CurrentView)); }
        }
    }
}
