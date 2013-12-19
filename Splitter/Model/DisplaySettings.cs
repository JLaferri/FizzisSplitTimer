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

namespace Fizzi.Applications.Splitter.Model
{
    [DataContract]
    class DisplaySettings : INotifyPropertyChanged
    {
        [DataMember(Name = "WindowWidth")]
        private double _windowWidth;
        public double WindowWidth { get { return _windowWidth; } set { this.RaiseAndSetIfChanged("WindowWidth", ref _windowWidth, value, PropertyChanged); } }

        [DataMember(Name = "WindowHeight")]
        private double _windowHeight;
        public double WindowHeight { get { return _windowHeight; } set { this.RaiseAndSetIfChanged("WindowHeight", ref _windowHeight, value, PropertyChanged); } }

        [DataMember(Name = "MainFont")]
        private FontDefinition _mainFont;
        public FontDefinition MainFont { get { return _mainFont; } set { this.RaiseAndSetIfChanged("MainFont", ref _mainFont, value, PropertyChanged); } }

        [DataMember(Name = "HeaderFont")]
        private FontDefinition _headerFont;
        public FontDefinition HeaderFont { get { return _headerFont; } set { this.RaiseAndSetIfChanged("HeaderFont", ref _headerFont, value, PropertyChanged); } }

        [DataMember(Name = "TimerFont")]
        private FontDefinition _timerFont;
        public FontDefinition TimerFont { get { return _timerFont; } set { this.RaiseAndSetIfChanged("TimerFont", ref _timerFont, value, PropertyChanged); } }

        public DisplaySettings()
        {
            WindowHeight = 400;
            WindowWidth = 250;

            MainFont = new FontDefinition()
            {
                Family = "Calibri",
                Size = 14
                //Weight = FontWeights.Normal,
                //Stretch = FontStretches.Normal,
                //Style = FontStyles.Normal
            };

            HeaderFont = new FontDefinition()
            {
                Family = "Calibri",
                Size = 14
            };

            TimerFont = new FontDefinition()
            {
                Family = "Calibri",
                Size = 40
            };
        }

        public DisplaySettings Clone()
        {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
