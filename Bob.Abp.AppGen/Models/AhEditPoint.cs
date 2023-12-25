using EnvDTE;
using Bob.Abp.AppGen.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob.Abp.AppGen.Models
{
    /// <summary>
    /// EditPoint in FileCodeModel.
    /// </summary>
    public class AhEditPoint : AhCodeElement
    {
        /// <summary>
        /// Position relative to the CodeElement
        /// </summary>
        public Positions Position { get; set; }

        /// <summary>
        /// Template's type.
        /// </summary>
        public TemplateType TemplateType {  get; set; }

        /// <summary>
        /// true means fall back to end of root element if the edit point is missing.
        /// </summary>
        public bool FallBackToRoot {  get; set; }

        public AhEditPoint(string name, vsCMElement kind, Positions position, TemplateType templateType, bool fallback = false)
            : base(name, kind)
        {
            Position = position;
            TemplateType = templateType;

            FallBackToRoot = fallback;
        }
    }
}
