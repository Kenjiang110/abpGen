using System;
using System.Collections.Generic;
using System.Linq;

namespace Bob.Abp.AppGen.Templates
{
    public class PropertyInfo : ICloneable
    {
        public string PropertyType { get; set; }

        public string DataFormat => ToDataFormat();

        /// <summary>
        /// Pascal format name
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Camel format name
        /// </summary>
        public string CamelName { get; set; }

        public bool Nullable { get; set; }  

        public bool PublicSetter { get; set; }

        public bool IsString { get; set; }

        public bool IsBoolean { get; set; }

        public bool IsDateTime { get; set; }

        public bool IsEnum { get; set; }

        public bool Required { get; set; }

        public bool IsDecimal { get; set; }

        public int? MaxLengthOrPercise { get; set; }

        public int? MinLengthOrScale { get; set; }

        public bool IsList { get; set; } = false;

        public bool IsHiddenField { get; set; } = true;

        public bool ReadOnlyWhenUpdate { get; set; } = false;

        public bool IsLast { get; set; } = false;

        public bool IsListField { get; set; } = false;

        public bool IsRequestField { get; set; } = false;

        public bool IsCreateField { get; set; } = true;

        public bool IsUpdateField { get; set; } = true;

        /// <summary>
        /// Members for Enum Type or Sub Entity Type
        /// </summary>
        public List<string> Members { get; set; }

        public string Member => Members?.FirstOrDefault();

        public List<string> RestMembers => Members?.Where(t => t != Member).ToList();

        public bool IsOverrideOrPureUpdateField => IsUpdateField && (IsCreateField && (IsHiddenField || ReadOnlyWhenUpdate) || !IsCreateField);

        public object Clone()
        {
            return MemberwiseClone();
        }

        private string ToDataFormat()
        {
            if (string.Compare(PropertyType, "bool", true) == 0)
            {
                return "boolean";
            }
            else if (string.Compare(PropertyType, "DateTime", true) ==  0)
            {
                return "datetime";
            }
            else
            {
                return PropertyType.ToLower();
            }
        }
    }
}
