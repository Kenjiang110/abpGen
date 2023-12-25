using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob.Abp.AppGen.Models
{
    /// <summary>
    /// AbpHelper's code element for insertion.
    /// Notice: For now, all core element to be inserted are <see cref="CodeElementType">sub element</see>.
    /// Thus, this class can only represent two kinds of code element,
    /// one (name is empty and kind is not function) is searched in <see cref="CodeElementType">root element</see>,
    /// anthor (name is not empty or kind is function) is searched in <see cref="CodeElementType">main element</see>.
    /// </summary>
    public class AhCodeElement
    {
        /// <summary>
        /// Code element's name. Different element kind has different name.
        /// For example name of "using namespace;" is namespace.
        /// </summary>
        public string Name { get; set; }
        private string NameTemplate { get; set; }

        /// <summary>
        /// Code element's kind.
        /// </summary>
        public vsCMElement Kind { get; set; }

        /// <summary>
        /// RootElement mean element not inside of class / interface
        /// </summary>
        public bool IsRootElement => string.IsNullOrEmpty(Name) && Kind != vsCMElement.vsCMElementFunction;

        public AhCodeElement(string name, vsCMElement kind)
        {
            Name = name;
            NameTemplate = name;
            Kind = kind;
        }

        /// <summary>
        /// Set RealName from moduleName and entityName.
        /// </summary>
        public void SetRealName(string moduleName, string entityName)
        {
            if (!String.IsNullOrEmpty(NameTemplate))
            {
                Name = string.Format(NameTemplate, moduleName, entityName);
                if (Name.EndsWith("_s"))
                {
                    Name = Utils.ToPlural(Name.TrimEnd("_s"));
                }
            }
            else
            {
                Name = NameTemplate;
            }
        }

        /// <summary>
        /// If Name is null and Kind = function means constructor.
        /// </summary>
        /// <param name="codeClassName">code class name is the constructor's name.</param>
        /// <returns></returns>
        public string GetRealName(string codeClassName)
        {
            if (string.IsNullOrEmpty(Name) && Kind == vsCMElement.vsCMElementFunction)
            {
                return codeClassName;
            }
            else
            {
                return Name;
            }
        }
    }
}
