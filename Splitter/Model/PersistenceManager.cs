using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.IO;

namespace Fizzi.Applications.Splitter.Model
{
    [DataContract]
    class PersistenceManager
    {
        #region Singleton Pattern Region
        private static volatile PersistenceManager instance;
        private static object syncRoot = new object();

        private PersistenceManager()
        {
            initialize();
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
        #endregion

        private string filePath;

        [DataMember]
        public ObservableCollection<DisplayTemplate> DisplayTemplates { get; private set; }

        private void initialize()
        {
            filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BananaSplits", "persistence.xml");

            if (!File.Exists(filePath))
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);

                var templatesConfigSection = config.GetSection("displayTemplates") as DisplayTemplatesConfigurationSection;

                if (templatesConfigSection != null && templatesConfigSection.DisplayTemplates != null)
                {
                    DisplayTemplates = new ObservableCollection<DisplayTemplate>(templatesConfigSection.DisplayTemplates.OfType<DisplayTemplate>());
                }

                Save();
            }
            else LoadFromStorage();

            if (DisplayTemplates == null) DisplayTemplates = new ObservableCollection<DisplayTemplate>();

            if (DisplayTemplates.Count == 0)
            {
                var defaultTemplate = new DisplayTemplate();
                defaultTemplate.TemplateName = "<Default>";

                DisplayTemplates.Add(defaultTemplate);

                Save();
            }
        }

        public void LoadFromStorage()
        {
            PersistenceManager result = null;

            try
            {
                DataContractSerializer dcs = new DataContractSerializer(typeof(PersistenceManager));

                using (Stream stream = new FileStream(filePath, FileMode.Open))
                {
                    result = (PersistenceManager)dcs.ReadObject(stream);
                }
            }
            catch (Exception) { }

            if (result != null)
            {
                DisplayTemplates = result.DisplayTemplates;
            }
        }

        public void Save()
        {
            var directory = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            DataContractSerializer dcs = new DataContractSerializer(typeof(PersistenceManager));

            using (Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                dcs.WriteObject(stream, this);
            }
        }
    }
}
