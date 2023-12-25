using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob.Abp.AppGen.Models
{
    public class PropertyGroup
    {
        public string Title { get; set; }

        public PropertyGroupType TabType { get; set; }

        public string Properties { get; set; } = String.Empty;

        public PropertyGroup(string title, PropertyGroupType tabType)
        {
            Title = title;
            TabType = tabType;
        }
    }
}
