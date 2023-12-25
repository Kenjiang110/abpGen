using Bob.Abp.AppGen.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob.Abp.AppGen.Dialogs.ViewModel
{
    public class PropertyGroupViewModel : NotifyClassBase
    {
        public string Title { get; set; }

        public PropertyGroupType TabType { get; set; }

        public string Properties { get; set; }

        public void SetProperties(List<KeyValue<string, string>> allPropertyNames)
        {
            var propertyNames = Properties.ToList(',');
            foreach (var name in allPropertyNames)
            {
                if (propertyNames.Contains(name.Value))
                {
                    Names.Add(name);
                }
                else
                {
                    Avalibles.Add(name);
                }
            }
        }

        public void CollectProperties()
        {
            Properties = string.Join(",", Names.Select(t=>t.Value));
        }

        public ObservableCollection<KeyValue<string, string>> Names { get; } = new ObservableCollection<KeyValue<string, string>>();

        public ObservableCollection<KeyValue<string, string>> Avalibles { get; } = new ObservableCollection<KeyValue<string, string>>();
    }
}
