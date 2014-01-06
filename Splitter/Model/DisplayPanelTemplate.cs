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

        public DisplayPanelTemplate()
        {
            BorderThickness = 2;
            BorderCornerRadius = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
