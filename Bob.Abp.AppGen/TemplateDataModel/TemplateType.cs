using Bob.Abp.AppGen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob.Abp.AppGen.Templates
{
    /// <summary>
    /// Template type enum
    /// </summary>
    public static class TemplateType
    {
        /// <summary>
        /// No template
        /// </summary>
        public const string None = "None";

        /// <summary>
        /// For core element.
        /// Main template's name is "[Abp File Type]_Main.template".
        /// </summary>
        public const string Main = "Main";

        /// <summary>
        /// For using core element.
        /// Calling template's name is "[Abp File Type]_Calling.template".
        /// </summary>
        public const string Calling = "Calling";

        /// <summary>
        /// Sometimes, Calling use Called to use core element undirectly.
        /// Calling template's name is "[Abp File Type]_Called.template".
        /// </summary>
        public const string Called = "Called";

        /// <summary>
        /// Namespace list for importing statements.
        /// Using template's name is "[Abp File Type]_Using.template".
        /// </summary>
        public const string Using = "Using";

        /// <summary>
        /// Element used to group variables using.
        /// Replace template's name is "[Abp File Type]_Group.template".
        /// </summary>
        public const string Group = "Group";

        public static bool Is(this AhEditPoint ahE, string typeName)
        {
            return ahE.TemplateName.EndsWith($"_{typeName}");
        }
    }
}
