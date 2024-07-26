using EnvDTE;
using System.Collections.Generic;

namespace Bob.Abp.AppGen.Models
{
    /// <summary>
    /// A project item to be edited.
    /// </summary>
    public class AhEditProjectItem : AhProjectItem
    {
        /// <summary>
        /// List of other edit points including core code element to be added to the source file
        /// </summary>
        public List<AhEditPoint> EditPoints { get; set; }

        /// <summary>
        /// Core code element to be added. Notice all core elements are <see cref="CodeElementType"/> sub element<see cref=""/>.
        /// Core element use template type "Main" to generate.
        /// </summary>
        public AhCodeElement CoreElement { get; set; }

        /// <summary>
        /// Contructor
        /// </summary>
        public AhEditProjectItem(AbpProjectType projectType, string relativePath, string fileName, vsCMElement kind, string name, string templateName, bool secured = false) : base(projectType, relativePath, fileName, templateName, secured)
        {
            EditPoints = new List<AhEditPoint>();
            RelativeFolder = relativePath;
            CoreElement = new AhCodeElement(name, kind);
        }

        /// <summary>
        /// Set all names.
        /// </summary>
        /// <param name="relativePath">ignored. Edit files are not relatived to entity relativePath.</param>
        /// <returns>This object for chain calling.</returns>
        public override AhProjectItem SetRealNames(string moduleName, string entityName, string relativePath = null)
        {
            base.SetRealNames(moduleName, entityName, null);
            CoreElement.SetRealName(moduleName, entityName);

            foreach (var editPoint in EditPoints)
            {
                editPoint.SetRealName(moduleName, entityName);
            }

            return this;
        }

        /// <summary>
        /// Add an edit point for this project item.
        /// </summary>
        /// <param name="name">null means use the most outer code element, for class or function that is file name without extension.</param>
        /// <param name="kind"></param>
        /// <param name="position"></param>
        /// <param name="tempType"></param>
        /// <returns>This object for chain calling.</returns>
        public AhEditProjectItem AddEditPoint(string name, vsCMElement kind, Positions position, string templateName, bool fallback = false)
        {
            var ep = new AhEditPoint(name, kind, position, $"{TemplateName}_{templateName}", fallback);
            EditPoints.Add(ep);

            return this;
        }
    }
}
