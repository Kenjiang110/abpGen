using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob.Abp.AppGen.Dialogs.ViewModel
{
    public class LanguageResourceViewModel : NotifyClassBase
    {
        public string Culture { get; set; }

        public string DisplayName { get; set; }

        public string NewEntityCmd { get; set; }

        /// <summary>
        /// Resources for properties.
        /// </summary>
        public Dictionary<string, string> Properties { get; set; }

        /// <summary>
        /// Resources for Enum.
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> Enums { get; set; }

        public void CollectProperties()
        {
            //collect properties
            Properties = new Dictionary<string, string>();
            foreach (var property in _propertyCollection)
            {
                Properties[property.Key] = property.Value;
            }
            //collect enums
            Enums = new Dictionary<string, Dictionary<string, string>>();
            foreach(var enumCollection in _enumCollection)
            {
                var enumDict = Enums[enumCollection.Key] = new Dictionary<string, string>();
                foreach(var enumItem in enumCollection.Value)
                {
                    enumDict[enumItem.Key] = enumItem.Value;
                }
            }
        }

        public ObservableCollection<KeyValue<string, string>> PropertyCollection
        {
            get
            {
                if (_propertyCollection == null)
                {
                    _propertyCollection = new ObservableCollection<KeyValue<string, string>>();
                    foreach (var property in Properties)
                    {
                        _propertyCollection.Add(new KeyValue<string, string> { Key = property.Key, Value = property.Value });
                    }
                }
                return _propertyCollection;
            }
        }
        private ObservableCollection<KeyValue<string, string>> _propertyCollection = null;

        public ObservableCollection<KeyValue<string, ObservableCollection<KeyValue<string, string>>>> EnumCollection
        {
            get
            {
                if (_enumCollection == null)
                {
                    _enumCollection = new ObservableCollection<KeyValue<string, ObservableCollection<KeyValue<string, string>>>>();
                    foreach (var enDict in Enums)
                    {
                        var enCollection = new KeyValue<string, ObservableCollection<KeyValue<string, string>>>
                        {
                            Key = enDict.Key,
                            Value = new ObservableCollection<KeyValue<string, string>>()
                        };
                        foreach (var enItem in enDict.Value)
                        {
                            enCollection.Value.Add(new KeyValue<string, string> { Key = enItem.Key, Value = enItem.Value });
                        }
                    }
                }
                return _enumCollection;
            }
        }
        private ObservableCollection<KeyValue<string, ObservableCollection<KeyValue<string, string>>>> _enumCollection = null;
    }
}
