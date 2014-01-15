using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using Fizzi.Applications.Splitter.Common;

namespace Fizzi.Applications.Splitter.Model
{
    class DisplayPanelTemplate : ConfigurationElement, INotifyPropertyChanged
    {
        [ConfigurationProperty("BorderCornerRadius", DefaultValue = 0d, IsRequired = true, IsKey = false)]
        public double BorderCornerRadius
        {
            get { return (double)this["BorderCornerRadius"]; }
            set
            {
                if (BorderCornerRadius != value)
                {
                    this["BorderCornerRadius"] = value;
                    this.Raise("BorderCornerRadius", PropertyChanged);
                }
            }
        }

        [ConfigurationProperty("BorderThickness", DefaultValue = 2d, IsRequired = true, IsKey = false)]
        public double BorderThickness
        {
            get { return (double)this["BorderThickness"]; }
            set
            {
                if (BorderThickness != value)
                {
                    this["BorderThickness"] = value;
                    this.Raise("BorderThickness", PropertyChanged);
                }
            }
        }

        [ConfigurationProperty("BackgroundColor", DefaultValue = "#303030")]
        public string BackgroundColor
        {
            get { return (string)this["BackgroundColor"]; }
            set
            {
                if (BackgroundColor != value)
                {
                    this["BackgroundColor"] = value;
                    this.Raise("BackgroundColor", PropertyChanged);
                }
            }
        }

        [ConfigurationProperty("BorderColor", DefaultValue = "White")]
        public string BorderColor
        {
            get { return (string)this["BorderColor"]; }
            set
            {
                if (BorderColor != value)
                {
                    this["BorderColor"] = value;
                    this.Raise("BorderColor", PropertyChanged);
                }
            }
        }

        [ConfigurationProperty("TextColor", DefaultValue = "White")]
        public string TextColor
        {
            get { return (string)this["TextColor"]; }
            set
            {
                if (TextColor != value)
                {
                    this["TextColor"] = value;
                    this.Raise("TextColor", PropertyChanged);
                }
            }
        }

        public DisplayPanelTemplate()
        {
            BorderThickness = 2;
            BorderCornerRadius = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
