using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob.Abp.AppGen.Dialogs.ViewModel
{
    public class PropertyViewModel : NotifyClassBase
    {
        public string PropertyType { get; set; }

        public string PropertyName { get; set; }

        public bool IsEnum { get; set; }

        public bool IsList { get; set; }

        public bool Nullable { get; set; }

        public bool PublicSetter { get; set; }

        public int? MaxLengthOrPercise { get; set; }

        public int? MinLengthOrScale { get; set; }

        public bool Required { get; set; }

        public bool IsHiddenField { get; set; }

        public bool ReadOnlyWhenUpdate { get; set; }

        public bool IsListField { get; set; }

        public bool IsRequestField { get; set; }

        public bool IsCreateField { get; set; }

        public bool IsUpdateField { get; set; }

        /// <summary>
        /// Members for Enum Type or Sub Entity Type
        /// </summary>
        public ObservableCollection<string> Members { get; set; }
    }
}
