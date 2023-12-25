using EnvDTE;
using Bob.Abp.AppGen.DteExtension;
using Bob.Abp.AppGen.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob.Abp.AppGen.Models
{
    /// <summary>
    /// Property Group Type
    /// </summary>
    public enum PropertyGroupType
    {
        BasicTab,

        MultiSelect,

        SimpleList
    }
}
