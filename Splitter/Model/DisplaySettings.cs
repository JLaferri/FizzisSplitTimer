using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Fizzi.Applications.Splitter.Common;
using System.Runtime.Serialization;
using System.Windows.Input;

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

        [DataMember]
        public Key SplitKey { get; set; }
        [DataMember]
        public Key UnsplitKey { get; set; }
        [DataMember]
        public Key SkipKey { get; set; }
        [DataMember]
        public Key ResetKey { get; set; }

        public DisplaySettings()
        {
            WindowHeight = 400;
            WindowWidth = 250;
        }

        public DisplaySettings Clone()
        {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
