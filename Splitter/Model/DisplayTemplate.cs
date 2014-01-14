using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fizzi.Windows.FontDialog;
using System.ComponentModel;
using System.Configuration;
using Fizzi.Applications.Splitter.Common;
using System.Windows.Media;

namespace Fizzi.Applications.Splitter.Model
{
    class DisplayTemplate : ConfigurationElement, INotifyPropertyChanged, IEditableObject
    {
        [ConfigurationProperty("TemplateId", IsRequired = true, IsKey = true)]
        public Guid TemplateId
        {
            get { return (Guid)this["TemplateId"]; }
            set { this["TemplateId"] = value; }
        }

        [ConfigurationProperty("TemplateName", DefaultValue = "Unnamed Display Template", IsRequired = true, IsKey = false)]
        public string TemplateName
        {
            get { return (string)this["TemplateName"]; }
            set 
            {
                if (TemplateName != value)
                {
                    this["TemplateName"] = value;
                    this.Raise("TemplateName", PropertyChanged);
                }
            }
        }

        [ConfigurationProperty("WindowWidth", DefaultValue=250d, IsRequired = true, IsKey = false)]
        public double WindowWidth
        {
            get { return (double)this["WindowWidth"]; }
            set
            {
                if (WindowWidth != value)
                {
                    this["WindowWidth"] = value;
                    this.Raise("WindowWidth", PropertyChanged);
                }
            }
        }

        [ConfigurationProperty("WindowHeight", DefaultValue=400d, IsRequired = true, IsKey = false)]
        public double WindowHeight
        {
            get { return (double)this["WindowHeight"]; }
            set
            {
                if (WindowHeight != value)
                {
                    this["WindowHeight"] = value;
                    this.Raise("WindowHeight", PropertyChanged);
                }
            }
        }

        [ConfigurationProperty("MainFont", IsRequired = true, IsKey = false)]
        public FontDefinition MainFont
        {
            get { return (FontDefinition)this["MainFont"]; }
            set
            {
                if (MainFont != value)
                {
                    this["MainFont"] = value;
                    this.Raise("MainFont", PropertyChanged);
                }
            }
        }

        [ConfigurationProperty("HeaderFont", IsRequired = true, IsKey = false)]
        public FontDefinition HeaderFont
        {
            get { return (FontDefinition)this["HeaderFont"]; }
            set
            {
                if (HeaderFont != value)
                {
                    this["HeaderFont"] = value;
                    this.Raise("HeaderFont", PropertyChanged);
                }
            }
        }

        [ConfigurationProperty("TimerFont", IsRequired = true, IsKey = false)]
        public FontDefinition TimerFont
        {
            get { return (FontDefinition)this["TimerFont"]; }
            set
            {
                if (TimerFont != value)
                {
                    this["TimerFont"] = value;
                    this.Raise("TimerFont", PropertyChanged);
                }
            }
        }

        [ConfigurationProperty("DefaultPanel", IsRequired = true, IsKey = false)]
        public DisplayPanelTemplate DefaultPanel
        {
            get { return (DisplayPanelTemplate)this["DefaultPanel"]; }
            set
            {
                if (DefaultPanel != value)
                {
                    this["DefaultPanel"] = value;
                    this.Raise("DefaultPanel", PropertyChanged);
                }
            }
        }

        [ConfigurationProperty("CurrentSplitPanel", IsRequired = true, IsKey = false)]
        public DisplayPanelTemplate CurrentSplitPanel
        {
            get { return (DisplayPanelTemplate)this["CurrentSplitPanel"]; }
            set
            {
                if (CurrentSplitPanel != value)
                {
                    this["CurrentSplitPanel"] = value;
                    this.Raise("CurrentSplitPanel", PropertyChanged);
                }
            }
        }

        [ConfigurationProperty("HeaderPanel", IsRequired = true, IsKey = false)]
        public DisplayPanelTemplate HeaderPanel
        {
            get { return (DisplayPanelTemplate)this["HeaderPanel"]; }
            set
            {
                if (HeaderPanel != value)
                {
                    this["HeaderPanel"] = value;
                    this.Raise("HeaderPanel", PropertyChanged);
                }
            }
        }

        [ConfigurationProperty("BackgroundColor", DefaultValue="Black")]
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

        public DisplayTemplate()
        {
            TemplateId = Guid.NewGuid();

            TemplateName = "Unnamed Display Template";

            WindowHeight = 400;
            WindowWidth = 250;

            MainFont = new FontDefinition();
            HeaderFont = new FontDefinition();
            TimerFont = new FontDefinition();
            TimerFont.Size = 40;

            DefaultPanel = new DisplayPanelTemplate();
            CurrentSplitPanel = new DisplayPanelTemplate();
            CurrentSplitPanel.BorderCornerRadius = 4;
            HeaderPanel = new DisplayPanelTemplate();

            BackgroundColor = "Black";
        }

        public DisplayTemplate Clone()
        {
            var newDisplayTemplate = new DisplayTemplate();

            newDisplayTemplate.TemplateName = TemplateName;

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

            newDisplayTemplate.DefaultPanel = new DisplayPanelTemplate()
            {
                BorderCornerRadius = DefaultPanel.BorderCornerRadius,
                BorderThickness = DefaultPanel.BorderThickness
            };

            newDisplayTemplate.CurrentSplitPanel = new DisplayPanelTemplate()
            {
                BorderCornerRadius = CurrentSplitPanel.BorderCornerRadius,
                BorderThickness = CurrentSplitPanel.BorderThickness
            };

            newDisplayTemplate.HeaderPanel = new DisplayPanelTemplate()
            {
                BorderCornerRadius = HeaderPanel.BorderCornerRadius,
                BorderThickness = HeaderPanel.BorderThickness
            };

            newDisplayTemplate.BackgroundColor = BackgroundColor;

            return newDisplayTemplate;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region IEditableObject Implementation

        public DisplayTemplate EditingBackup { get; private set; }

        public void BeginEdit()
        {
            EditingBackup = this.Clone();
        }

        public void CancelEdit()
        {
            //Revert all changes back to what they were when editing began
            if (EditingBackup == null) return;

            TemplateName = EditingBackup.TemplateName;

            WindowHeight = EditingBackup.WindowHeight;
            WindowWidth = EditingBackup.WindowWidth;

            MainFont.Family = EditingBackup.MainFont.Family;
            MainFont.Size = EditingBackup.MainFont.Size;

            HeaderFont.Family = EditingBackup.HeaderFont.Family;
            HeaderFont.Size = EditingBackup.HeaderFont.Size;

            TimerFont.Family = EditingBackup.TimerFont.Family;
            TimerFont.Size = EditingBackup.TimerFont.Size;

            DefaultPanel.BorderCornerRadius = EditingBackup.DefaultPanel.BorderCornerRadius;
            DefaultPanel.BorderThickness = EditingBackup.DefaultPanel.BorderThickness;

            CurrentSplitPanel.BorderCornerRadius = EditingBackup.CurrentSplitPanel.BorderCornerRadius;
            CurrentSplitPanel.BorderThickness = EditingBackup.CurrentSplitPanel.BorderThickness;

            HeaderPanel.BorderCornerRadius = EditingBackup.HeaderPanel.BorderCornerRadius;
            HeaderPanel.BorderThickness = EditingBackup.HeaderPanel.BorderThickness;

            BackgroundColor = EditingBackup.BackgroundColor;

            EditingBackup = null;
        }

        public void EndEdit()
        {
            //Keep all changes that were made to the object and clear editing state
            PersistenceManager.Instance.DisplayTemplatesConfiguration.Save();

            EditingBackup = null;
        }

        #endregion
    }
}
