using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Collections.Specialized;

namespace Fizzi.Applications.Splitter.Model
{
    class PersistenceManager
    {
        //Singleton class - exposes ObservableCollections and keeps them synchronized with ConfigurationSections
        private static volatile PersistenceManager instance;
        private static object syncRoot = new object();

        private PersistenceManager()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);

            templatesConfigSection = config.GetSection("displayTemplates") as DisplayTemplatesConfigurationSection;

            DisplayTemplates = new ObservableCollection<DisplayTemplate>(templatesConfigSection.DisplayTemplates.OfType<DisplayTemplate>());

            DisplayTemplates.CollectionChanged += (sender, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add) templatesConfigSection.DisplayTemplates.Add((DisplayTemplate)e.NewItems[0]);
                else if (e.Action == NotifyCollectionChangedAction.Remove) templatesConfigSection.DisplayTemplates.Remove((DisplayTemplate)e.OldItems[0]);
                else if (e.Action == NotifyCollectionChangedAction.Reset) templatesConfigSection.DisplayTemplates.Clear();
            };
        }

        public static PersistenceManager Instance
        {
            get 
            {
                if (instance == null) 
                {
                    lock (syncRoot) 
                    {
                        if (instance == null) instance = new PersistenceManager();
                    }
                }

                return instance;
            }
        }

        private DisplayTemplatesConfigurationSection templatesConfigSection;

        public ObservableCollection<DisplayTemplate> DisplayTemplates { get; private set; }
        public Configuration DisplayTemplatesConfiguration { get { return templatesConfigSection.CurrentConfiguration; } }
    }
}
