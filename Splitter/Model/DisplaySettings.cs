using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Fizzi.Applications.Splitter.Common;
using System.Runtime.Serialization;
using System.Windows.Input;
using Fizzi.Windows.FontDialog;
using System.Windows.Media;
using System.Windows;
using System.Xml.Serialization;

namespace Fizzi.Applications.Splitter.Model
{
    [DataContract]
    [Obsolete("DisplayTemplate Class should be used instead of this. This class must continue to exist for deserialization of old .fsp files.", false)]
    class DisplaySettings : INotifyPropertyChanged
    {
        [DataMember]
        public Guid TemplateId { get; private set; }

        [DataMember(Name = "TemplateName")]
        private string _templateName;
        public string TemplateName { get { return _templateName; } set { this.RaiseAndSetIfChanged("TemplateName", ref _templateName, value, PropertyChanged); } }

        [DataMember(Name = "WindowWidth")]
        private double _windowWidth;
        public double WindowWidth { get { return _windowWidth; } set { this.RaiseAndSetIfChanged("WindowWidth", ref _windowWidth, value, PropertyChanged); } }

        [DataMember(Name = "WindowHeight")]
        private double _windowHeight;
        public double WindowHeight { get { return _windowHeight; } set { this.RaiseAndSetIfChanged("WindowHeight", ref _windowHeight, value, PropertyChanged); } }

        [DataMember(Name = "MainFont")]
        private Old_FontDefinition _mainFont;
        public Old_FontDefinition MainFont { get { return _mainFont; } set { this.RaiseAndSetIfChanged("MainFont", ref _mainFont, value, PropertyChanged); } }

        [DataMember(Name = "HeaderFont")]
        private Old_FontDefinition _headerFont;
        public Old_FontDefinition HeaderFont { get { return _headerFont; } set { this.RaiseAndSetIfChanged("HeaderFont", ref _headerFont, value, PropertyChanged); } }

        [DataMember(Name = "TimerFont")]
        private Old_FontDefinition _timerFont;
        public Old_FontDefinition TimerFont { get { return _timerFont; } set { this.RaiseAndSetIfChanged("TimerFont", ref _timerFont, value, PropertyChanged); } }

        public DisplaySettings()
        {
            TemplateId = Guid.NewGuid();

            TemplateName = "Unnamed Display Template";

            WindowHeight = 400;
            WindowWidth = 250;

            MainFont = new Old_FontDefinition()
            {
                Family = "Calibri",
                Size = 14
                //Weight = FontWeights.Normal,
                //Stretch = FontStretches.Normal,
                //Style = FontStyles.Normal
            };

            HeaderFont = new Old_FontDefinition()
            {
                Family = "Calibri",
                Size = 14
            };
            
            TimerFont = new Old_FontDefinition()
            {
                Family = "Calibri",
                Size = 40
            };
        }

        public DisplayTemplate ConvertToTemplate()
        {
            var newDisplayTemplate = new DisplayTemplate();

            newDisplayTemplate.TemplateName = "(Copy) " + TemplateName;

            newDisplayTemplate.WindowHeight = WindowHeight;
            newDisplayTemplate.WindowWidth = WindowWidth;

            newDisplayTemplate.MainFont = new FontDefinition()
            {
                Family = MainFont.Family,
                Size = MainFont.Size
            };

            newDisplayTemplate.HeaderFont = new FontDefinition()
            {
                Family = HeaderFont.Family,
                Size = HeaderFont.Size
            };

            newDisplayTemplate.TimerFont = new FontDefinition()
            {
                Family = TimerFont.Family,
                Size = TimerFont.Size
            };

            return newDisplayTemplate;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
