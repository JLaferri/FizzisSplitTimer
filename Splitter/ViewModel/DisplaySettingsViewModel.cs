using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Collections.ObjectModel;
using Fizzi.Applications.Splitter.Model;
using Fizzi.Applications.Splitter.Properties;
using Fizzi.Applications.Splitter.Common;
using System.ComponentModel;
using System.Windows.Input;

namespace Fizzi.Applications.Splitter.ViewModel
{
    class DisplayTemplatesViewModel : INotifyPropertyChanged
    {
        public string[] AvailableFonts { get; private set; }

        public ICommand CloneTemplate { get; private set; }
        public ICommand RemoveTemplate { get; private set; }

        public ObservableCollection<DisplayTemplate> UserTemplates { get; private set; }
        public DisplayTemplate DefaultDisplayTemplate { get; private set; }

        private DisplayTemplate _selectedDisplayTemplate;
        public DisplayTemplate SelectedDisplayTemplate { get { return _selectedDisplayTemplate; } set { this.RaiseAndSetIfChanged("SelectedDisplayTemplate", ref _selectedDisplayTemplate, value, PropertyChanged); } }

        private readonly MainViewModel mainViewModel;

        public DisplayTemplatesViewModel(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;

            AvailableFonts = Fonts.SystemFontFamilies.Select(ff => ff.ToString()).OrderBy(s => s).ToArray();

            UserTemplates = PersistenceManager.Instance.DisplayTemplates;
            DefaultDisplayTemplate = UserTemplates.FirstOrDefault();
            SelectedDisplayTemplate = DefaultDisplayTemplate;

            CloneTemplate = Command.Create(() => true, () =>
            {
                if (SelectedDisplayTemplate == null) return;

                var cloned = SelectedDisplayTemplate.Clone();
                cloned.TemplateName = "(Copy) " + cloned.TemplateName;
                PersistenceManager.Instance.DisplayTemplates.Add(cloned);

                SelectedDisplayTemplate = cloned;
            });

            RemoveTemplate = Command.Create(() => true, () =>
            {
                if (SelectedDisplayTemplate == null) return;

                PersistenceManager.Instance.DisplayTemplates.Remove(SelectedDisplayTemplate);

                SelectedDisplayTemplate = DefaultDisplayTemplate;
            });

            //Keep file template synchronized with selected template
            this.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "SelectedDisplayTemplate")
                {
                    if (mainViewModel.CurrentFile != null) mainViewModel.CurrentFile.DisplayTemplate = SelectedDisplayTemplate;
                }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
