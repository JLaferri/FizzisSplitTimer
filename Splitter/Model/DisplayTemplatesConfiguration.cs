using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Runtime.Serialization;

namespace Fizzi.Applications.Splitter.Model
{
    class DisplayTemplatesConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("available", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(DisplayTemplateCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public DisplayTemplateCollection DisplayTemplates
        {
            get
            {
                return (DisplayTemplateCollection)base["available"];
            }
        }
    }

    class DisplayTemplateCollection : ConfigurationElementCollection
    {
        public DisplayTemplate this[int index]
        {
            get { return (DisplayTemplate)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(DisplayTemplate element)
        {
            BaseAdd(element);
        }

        public void Clear()
        {
            BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new DisplayTemplate();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DisplayTemplate)element).TemplateId;
        }

        public void Remove(DisplayTemplate element)
        {
            BaseRemove(element.TemplateId);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public override bool IsReadOnly()
        {
            return false;
        }
    }
}
