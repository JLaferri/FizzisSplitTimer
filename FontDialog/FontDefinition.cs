using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using Fizzi.Windows.FontDialog.Common;

namespace Fizzi.Windows.FontDialog
{
    public class FontDefinition : ConfigurationElement, INotifyPropertyChanged
    {
        [ConfigurationProperty("Family", DefaultValue = "Calibri", IsRequired = true, IsKey = false)]
        public string Family
        {
            get { return (string)this["Family"]; }
            set
            {
                if (Family != value)
                {
                    this["Family"] = value;
                    this.Raise("Family", PropertyChanged);
                }
            }
        }

        [ConfigurationProperty("Size", DefaultValue = 14d, IsRequired = true, IsKey = false)]
        public double Size
        {
            get { return (double)this["Size"]; }
            set
            {
                if (Size != value)
                {
                    this["Size"] = value;
                    this.Raise("Size", PropertyChanged);
                }
            }
        }

        //[DataMember(Name = "Style")]
        //private FontStyle _style;
        //public FontStyle Style { get { return _style; } set { this.RaiseAndSetIfChanged("Style", ref _style, value, PropertyChanged); } }

        //[DataMember(Name = "Stretch")]
        //private FontStretch _stretch;
        //public FontStretch Stretch { get { return _stretch; } set { this.RaiseAndSetIfChanged("Stretch", ref _stretch, value, PropertyChanged); } }

        //[DataMember(Name = "Weight")]
        //private FontWeight _weight;
        //public FontWeight Weight { get { return _weight; } set { this.RaiseAndSetIfChanged("Weight", ref _weight, value, PropertyChanged); } }

        public FontDefinition()
        {
            Family = "Calibri";
            Size = 14;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
