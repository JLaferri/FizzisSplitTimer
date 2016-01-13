using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using Fizzi.Applications.Splitter.Common;
using System.Runtime.Serialization;

namespace Fizzi.Applications.Splitter.Model
{
    [DataContract]
    class DisplayPanelTemplate : ConfigurationElement, INotifyPropertyChanged
    {
        [DataMember]
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

        [DataMember]
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

        [DataMember]
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

        [DataMember]
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

        [DataMember]
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
            BackgroundColor = "#303030";
            BorderColor = "White";
            TextColor = "White";
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
