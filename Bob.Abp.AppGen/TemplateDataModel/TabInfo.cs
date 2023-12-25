using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob.Abp.AppGen.Templates
{
    public class TabInfo
    {
        public string Title { get; set; }

        public bool IsBasic { get; set; } = false;

        public bool IsMultiSelect { get; set; } = false;

        public bool IsSimpleList { get; set; } = false;

        public bool IsUpdateTab { get; set; } = true;

        public bool IsCreateTab { get; set; } = true;

        public List<PropertyInfo> Properties { get; set; }

        public PropertyInfo Property => Properties?.FirstOrDefault();
    }
}
