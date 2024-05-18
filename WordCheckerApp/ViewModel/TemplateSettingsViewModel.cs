using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows.Input;
using WordCheckerApp.Model;

namespace WordCheckerApp.ViewModel
{
    public class TemplateSettingsViewModel : ViewModelBase
    {
        private ObservableCollection<string> _templateNames;
        private string _selectedTemplate;
        private string _templateName;
        private ObservableCollection<HeaderSetting> _headers;
        private string _mainTextFont;
        private string _headerFont;
        private double _marginTop;
        private double _marginBottom;
        private double _marginLeft;
        private double _marginRight;
        private double _indentation;

        public ObservableCollection<string> TemplateNames
        {
            get => _templateNames;
            set => SetProperty(ref _templateNames, value);
        }

        public string SelectedTemplate
        {
            get => _selectedTemplate;
            set => SetProperty(ref _selectedTemplate, value);
        }

        public string TemplateName
        {
            get => _templateName;
            set => SetProperty(ref _templateName, value);
        }

        public ObservableCollection<HeaderSetting> Headers
        {
            get => _headers;
            set => SetProperty(ref _headers, value);
        }

        public string MainTextFont
        {
            get => _mainTextFont;
            set => SetProperty(ref _mainTextFont, value);
        }

        public string HeaderFont
        {
            get => _headerFont;
            set => SetProperty(ref _headerFont, value);
        }

        public double MarginTop
        {
            get => _marginTop;
            set => SetProperty(ref _marginTop, value);
        }

        public double MarginBottom
        {
            get => _marginBottom;
            set => SetProperty(ref _marginBottom, value);
        }

        public double MarginLeft
        {
            get => _marginLeft;
            set => SetProperty(ref _marginLeft, value);
        }

        public double MarginRight
        {
            get => _marginRight;
            set => SetProperty(ref _marginRight, value);
        }

        public double Indentation
        {
            get => _indentation;
            set => SetProperty(ref _indentation, value);
        }

        public ICommand LoadTemplateCommand { get; }
        public ICommand SaveTemplateCommand { get; }

        public TemplateSettingsViewModel()
        {
            Headers = new ObservableCollection<HeaderSetting>();
            TemplateNames = new ObservableCollection<string>();
            LoadTemplateCommand = new RelayCommand(_ => LoadTemplate());
            SaveTemplateCommand = new RelayCommand(_ => SaveTemplate());
            LoadTemplates(); // Initialize by loading available templates
        }

        private void LoadTemplates()
        {
            TemplateNames.Clear();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Resources\Templates");
            foreach (var filePath in Directory.GetFiles(path, "*.json"))
            {
                string json = File.ReadAllText(filePath);
                var template = JsonSerializer.Deserialize<TemplateSettings>(json);
                TemplateNames.Add(template.TemplateName);
            }
        }

        private void LoadTemplate()
        {
            if (string.IsNullOrEmpty(SelectedTemplate)) return;

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Resources\Templates", $"{SelectedTemplate}.json");
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                var template = JsonSerializer.Deserialize<TemplateSettings>(json);
                TemplateName = template.TemplateName;
                Headers = new ObservableCollection<HeaderSetting>(template.Headers);
                MainTextFont = template.MainTextFont;
                HeaderFont = template.HeaderFont;
                MarginTop = template.MarginTop;
                MarginBottom = template.MarginBottom;
                MarginLeft = template.MarginLeft;
                MarginRight = template.MarginRight;
                Indentation = template.Indentation;
            }
        }

        private void SaveTemplate()
        {
            var template = new TemplateSettings
            {
                TemplateName = TemplateName,
                Headers = new List<HeaderSetting>(Headers),
                MainTextFont = MainTextFont,
                HeaderFont = HeaderFont,
                MarginTop = MarginTop,
                MarginBottom = MarginBottom,
                MarginLeft = MarginLeft,
                MarginRight = MarginRight,
                Indentation = Indentation
            };

            string json = JsonSerializer.Serialize(template);
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Resources\Templates", $"{TemplateName}.json");
            File.WriteAllText(path, json);
        }
    }
}
