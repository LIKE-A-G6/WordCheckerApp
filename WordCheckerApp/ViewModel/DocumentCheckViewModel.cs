using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Input;
using Microsoft.Win32;
using WordCheckerApp.Model;
using System;
using DocumentFormat.OpenXml;

namespace WordCheckerApp.ViewModel
{
    public class DocumentCheckViewModel : ViewModelBase
    {
        private string loadedDocumentPath;
        public ObservableCollection<string> TemplateNames { get; set; }
        public string SelectedTemplate { get; set; }
        public string CheckResults { get; set; }

        public ICommand LoadDocumentCommand { get; }
        public ICommand CheckDocumentCommand { get; }
        public ICommand FixIssuesCommand { get; }

        public DocumentCheckViewModel()
        {
            TemplateNames = new ObservableCollection<string>();
            LoadTemplates();
            LoadDocumentCommand = new RelayCommand(param => LoadDocument());
            CheckDocumentCommand = new RelayCommand(param => CheckDocument());
            FixIssuesCommand = new RelayCommand(param => FixIssues());
        }

        private void LoadTemplates()
        {
            // Определение пути к папке Templates в проекте
            string projectDirectory = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.FullName;
            string templatesPath = Path.Combine(projectDirectory, "Resources", "Templates");

            if (Directory.Exists(templatesPath))
            {
                var files = Directory.GetFiles(templatesPath, "*.json");
                foreach (var file in files)
                {
                    var template = JsonSerializer.Deserialize<TemplateSettings>(File.ReadAllText(file));
                    TemplateNames.Add(template.TemplateName);
                }
            }
        }

        private void LoadDocument()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Word Documents|*.docx"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                loadedDocumentPath = openFileDialog.FileName;
            }
        }

        private void CheckDocument()
        {
            if (string.IsNullOrEmpty(loadedDocumentPath) || string.IsNullOrEmpty(SelectedTemplate))
            {
                CheckResults = "Пожалуйста, выберите шаблон и загрузите документ.";
                OnPropertyChanged(nameof(CheckResults));
                return;
            }

            // Определение пути к папке Templates в проекте
            string projectDirectory = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.FullName;
            string templatePath = Path.Combine(projectDirectory, "Resources", "Templates", $"{SelectedTemplate}.json");
            var templateSettings = JsonSerializer.Deserialize<TemplateSettings>(File.ReadAllText(templatePath));

            var issues = PerformDocumentCheck(loadedDocumentPath, templateSettings);
            CheckResults = string.Join("\n", issues);
            OnPropertyChanged(nameof(CheckResults));
        }

        private List<string> PerformDocumentCheck(string documentPath, TemplateSettings templateSettings)
        {
            var issues = new List<string>();

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(documentPath, false))
            {
                var paragraphs = wordDoc.MainDocumentPart.Document.Body.Elements<Paragraph>();

                // Проверка заголовков
                foreach (var headerSetting in templateSettings.Headers)
                {
                    bool headerExists = paragraphs.Any(p => p.ParagraphProperties?.ParagraphStyleId?.Val == headerSetting.HeaderStyle);

                    if (!headerExists)
                    {
                        issues.Add($"Заголовок '{headerSetting.HeaderText}' со стилем '{headerSetting.HeaderStyle}' отсутствует.");
                    }
                }

                // Проверка других настроек...
            }

            return issues;
        }

        private void FixIssues()
        {
            if (string.IsNullOrEmpty(loadedDocumentPath) || string.IsNullOrEmpty(SelectedTemplate))
            {
                CheckResults = "Пожалуйста, выберите шаблон и загрузите документ.";
                OnPropertyChanged(nameof(CheckResults));
                return;
            }

            // Определение пути к папке Templates в проекте
            string projectDirectory = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.FullName;
            string templatePath = Path.Combine(projectDirectory, "Resources", "Templates", $"{SelectedTemplate}.json");
            var templateSettings = JsonSerializer.Deserialize<TemplateSettings>(File.ReadAllText(templatePath));
            ApplyFixesToDocument(loadedDocumentPath, templateSettings);
        }

        private void ApplyFixesToDocument(string documentPath, TemplateSettings templateSettings)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(documentPath, true))
            {
                var paragraphs = wordDoc.MainDocumentPart.Document.Body.Elements<Paragraph>();

                foreach (var headerSetting in templateSettings.Headers)
                {
                    bool headerExists = paragraphs.Any(p => p.ParagraphProperties?.ParagraphStyleId?.Val == headerSetting.HeaderStyle);

                    if (!headerExists)
                    {
                        Paragraph headerParagraph = new Paragraph(new Run(new Text(headerSetting.HeaderText)))
                        {
                            ParagraphProperties = new ParagraphProperties
                            {
                                ParagraphStyleId = new ParagraphStyleId { Val = headerSetting.HeaderStyle }
                            }
                        };
                        wordDoc.MainDocumentPart.Document.Body.InsertAt(headerParagraph, 0);
                    }
                }

                var sections = wordDoc.MainDocumentPart.Document.Body.Elements<SectionProperties>();
                if (sections.Any())
                {
                    var section = sections.First();
                    int marginTopInTwips = (int)(templateSettings.MarginTop * 0.393701 * 1440);
                    int marginBottomInTwips = (int)(templateSettings.MarginBottom * 0.393701 * 1440);
                    int marginLeftInTwips = (int)(templateSettings.MarginLeft * 0.393701 * 1440);
                    int marginRightInTwips = (int)(templateSettings.MarginRight * 0.393701 * 1440);

                    var pageMargin = new PageMargin
                    {
                        Top = new Int32Value(marginTopInTwips),
                        Bottom = new Int32Value(marginBottomInTwips),
                        Left = new UInt32Value((uint)marginLeftInTwips),
                        Right = new UInt32Value((uint)marginRightInTwips)
                    };
                    section.Append(pageMargin);
                }

                wordDoc.MainDocumentPart.Document.Save();
            }
        }
    }
}
