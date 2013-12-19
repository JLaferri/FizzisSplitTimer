using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.ComponentModel;
using Fizzi.Windows.FontDialog.Common;
using System.Windows;

namespace Fizzi.Windows.FontDialog
{
    [DataContract]
    public class FontDefinition : INotifyPropertyChanged
    {
        [DataMember(Name="Family")]
        private string _family;
        public string Family { get { return _family; } set { this.RaiseAndSetIfChanged("Family", ref _family, value, PropertyChanged); } }

        [DataMember(Name = "Size")]
        private double _size;
        public double Size { get { return _size; } set { this.RaiseAndSetIfChanged("Size", ref _size, value, PropertyChanged); } }

        //[DataMember(Name = "Style")]
        //private FontStyle _style;
        //public FontStyle Style { get { return _style; } set { this.RaiseAndSetIfChanged("Style", ref _style, value, PropertyChanged); } }

        //[DataMember(Name = "Stretch")]
        //private FontStretch _stretch;
        //public FontStretch Stretch { get { return _stretch; } set { this.RaiseAndSetIfChanged("Stretch", ref _stretch, value, PropertyChanged); } }

        //[DataMember(Name = "Weight")]
        //private FontWeight _weight;
        //public FontWeight Weight { get { return _weight; } set { this.RaiseAndSetIfChanged("Weight", ref _weight, value, PropertyChanged); } }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
