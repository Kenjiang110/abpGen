using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob.Abp.AppGen.Models
{
    public class EntityProperty
    {
        public string PropertyType { get; set; }

        public string PropertyName { get; set; }

        public bool Nullable { get; set; }

        public bool IsEnum { get; set; } = false;

        public bool PublicSetter { get; set; } = true;

        public bool IsList { get; set; } = false;

        public int? MaxLengthOrPercise { get; set; }

        public int? MinLengthOrScale { get; set; }

        public bool Required { get; set; } = false;

        /// <summary>
        /// Members for Enum Type or Sub Entity Type
        /// </summary>
        public List<string> Members { get; set; }
    }
}
