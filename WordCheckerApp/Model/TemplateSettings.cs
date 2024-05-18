using System.Collections.Generic;

namespace WordCheckerApp.Model
{
    public class TemplateSettings
    {
        public string TemplateName { get; set; }
        public List<HeaderSetting> Headers { get; set; }
        public string MainTextFont { get; set; }
        public string HeaderFont { get; set; }
        public double MarginTop { get; set; }
        public double MarginBottom { get; set; }
        public double MarginLeft { get; set; }
        public double MarginRight { get; set; }
        public double Indentation { get; set; }
    }
}
