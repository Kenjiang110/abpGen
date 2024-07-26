using EnvDTE;

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
        public string TemplateName { get; set; }

        /// <summary>
        /// true means fall back to end of root element if the edit point is missing.
        /// </summary>
        public bool FallBackToRoot { get; set; }

        public AhEditPoint(string name, vsCMElement kind, Positions position, string templateName, bool fallback = false)
            : base(name, kind)
        {
            Position = position;
            TemplateName = templateName;

            FallBackToRoot = fallback;
        }
    }
}
