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

        [ConfigurationProperty("GoldPanel", IsRequired = true, IsKey = false)]
        public DisplayPanelTemplate GoldPanel
        {
            get { return (DisplayPanelTemplate)this["GoldPanel"]; }
            set
            {
                if (GoldPanel != value)
                {
                    this["GoldPanel"] = value;
                    this.Raise("GoldPanel", PropertyChanged);
                }
            }
        }

        [ConfigurationProperty("AheadPanel", IsRequired = true, IsKey = false)]
        public DisplayPanelTemplate AheadPanel
        {
            get { return (DisplayPanelTemplate)this["AheadPanel"]; }
            set
            {
                if (AheadPanel != value)
                {
                    this["AheadPanel"] = value;
                    this.Raise("AheadPanel", PropertyChanged);
                }
            }
        }

        [ConfigurationProperty("BehindPanel", IsRequired = true, IsKey = false)]
        public DisplayPanelTemplate BehindPanel
        {
            get { return (DisplayPanelTemplate)this["BehindPanel"]; }
            set
            {
                if (BehindPanel != value)
                {
                    this["BehindPanel"] = value;
                    this.Raise("BehindPanel", PropertyChanged);
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

        [ConfigurationProperty("TimerColor", DefaultValue = "White")]
        public string TimerColor
        {
            get { return (string)this["TimerColor"]; }
            set
            {
                if (TimerColor != value)
                {
                    this["TimerColor"] = value;
                    this.Raise("TimerColor", PropertyChanged);
                }
            }
        }

        protected override void InitializeDefault()
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

            GoldPanel = new DisplayPanelTemplate();
            GoldPanel.BorderColor = "Gold";
            AheadPanel = new DisplayPanelTemplate();
            AheadPanel.BorderColor = "LimeGreen";
            BehindPanel = new DisplayPanelTemplate();
            BehindPanel.BorderColor = "Red";

            BackgroundColor = "Black";
            TimerColor = "White";
        }

        public DisplayTemplate()
        {
            InitializeDefault();
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
                BorderThickness = DefaultPanel.BorderThickness,
                BackgroundColor = DefaultPanel.BackgroundColor,
                BorderColor = DefaultPanel.BorderColor,
                TextColor = DefaultPanel.TextColor
            };

            newDisplayTemplate.CurrentSplitPanel = new DisplayPanelTemplate()
            {
                BorderCornerRadius = CurrentSplitPanel.BorderCornerRadius,
                BorderThickness = CurrentSplitPanel.BorderThickness,
                BackgroundColor = CurrentSplitPanel.BackgroundColor,
                BorderColor = CurrentSplitPanel.BorderColor,
                TextColor = CurrentSplitPanel.TextColor
            };

            newDisplayTemplate.HeaderPanel = new DisplayPanelTemplate()
            {
                BorderCornerRadius = HeaderPanel.BorderCornerRadius,
                BorderThickness = HeaderPanel.BorderThickness,
                BackgroundColor = HeaderPanel.BackgroundColor,
                BorderColor = HeaderPanel.BorderColor,
                TextColor = HeaderPanel.TextColor
            };

            newDisplayTemplate.GoldPanel = new DisplayPanelTemplate()
            {
                BorderCornerRadius = GoldPanel.BorderCornerRadius,
                BorderThickness = GoldPanel.BorderThickness,
                BackgroundColor = GoldPanel.BackgroundColor,
                BorderColor = GoldPanel.BorderColor,
                TextColor = GoldPanel.TextColor
            };

            newDisplayTemplate.AheadPanel = new DisplayPanelTemplate()
            {
                BorderCornerRadius = AheadPanel.BorderCornerRadius,
                BorderThickness = AheadPanel.BorderThickness,
                BackgroundColor = AheadPanel.BackgroundColor,
                BorderColor = AheadPanel.BorderColor,
                TextColor = AheadPanel.TextColor
            };

            newDisplayTemplate.BehindPanel = new DisplayPanelTemplate()
            {
                BorderCornerRadius = BehindPanel.BorderCornerRadius,
                BorderThickness = BehindPanel.BorderThickness,
                BackgroundColor = BehindPanel.BackgroundColor,
                BorderColor = BehindPanel.BorderColor,
                TextColor = BehindPanel.TextColor
            };

            newDisplayTemplate.BackgroundColor = BackgroundColor;
            newDisplayTemplate.TimerColor = TimerColor;

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
            DefaultPanel.BackgroundColor = EditingBackup.DefaultPanel.BackgroundColor;
            DefaultPanel.BorderColor = EditingBackup.DefaultPanel.BorderColor;
            DefaultPanel.TextColor = EditingBackup.DefaultPanel.TextColor;

            CurrentSplitPanel.BorderCornerRadius = EditingBackup.CurrentSplitPanel.BorderCornerRadius;
            CurrentSplitPanel.BorderThickness = EditingBackup.CurrentSplitPanel.BorderThickness;
            CurrentSplitPanel.BackgroundColor = EditingBackup.CurrentSplitPanel.BackgroundColor;
            CurrentSplitPanel.BorderColor = EditingBackup.CurrentSplitPanel.BorderColor;
            CurrentSplitPanel.TextColor = EditingBackup.CurrentSplitPanel.TextColor;

            HeaderPanel.BorderCornerRadius = EditingBackup.HeaderPanel.BorderCornerRadius;
            HeaderPanel.BorderThickness = EditingBackup.HeaderPanel.BorderThickness;
            HeaderPanel.BackgroundColor = EditingBackup.HeaderPanel.BackgroundColor;
            HeaderPanel.BorderColor = EditingBackup.HeaderPanel.BorderColor;
            HeaderPanel.TextColor = EditingBackup.HeaderPanel.TextColor;

            GoldPanel.BorderCornerRadius = EditingBackup.GoldPanel.BorderCornerRadius;
            GoldPanel.BorderThickness = EditingBackup.GoldPanel.BorderThickness;
            GoldPanel.BackgroundColor = EditingBackup.GoldPanel.BackgroundColor;
            GoldPanel.BorderColor = EditingBackup.GoldPanel.BorderColor;
            GoldPanel.TextColor = EditingBackup.GoldPanel.TextColor;

            AheadPanel.BorderCornerRadius = EditingBackup.AheadPanel.BorderCornerRadius;
            AheadPanel.BorderThickness = EditingBackup.AheadPanel.BorderThickness;
            AheadPanel.BackgroundColor = EditingBackup.AheadPanel.BackgroundColor;
            AheadPanel.BorderColor = EditingBackup.AheadPanel.BorderColor;
            AheadPanel.TextColor = EditingBackup.AheadPanel.TextColor;

            BehindPanel.BorderCornerRadius = EditingBackup.BehindPanel.BorderCornerRadius;
            BehindPanel.BorderThickness = EditingBackup.BehindPanel.BorderThickness;
            BehindPanel.BackgroundColor = EditingBackup.BehindPanel.BackgroundColor;
            BehindPanel.BorderColor = EditingBackup.BehindPanel.BorderColor;
            BehindPanel.TextColor = EditingBackup.BehindPanel.TextColor;

            BackgroundColor = EditingBackup.BackgroundColor;
            TimerColor = EditingBackup.TimerColor;

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
