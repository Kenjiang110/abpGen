using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob.Abp.AppGen.Models
{
    public class LanguageResource
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
    }
}
