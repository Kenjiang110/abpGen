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
    public enum TemplateType
    {
        /// <summary>
        /// No template
        /// </summary>
        None,

        /// <summary>
        /// For core element.
        /// Main template's name is "[Abp File Type].template".
        /// </summary>
        Main,

        /// <summary>
        /// For using core element.
        /// Calling template's name is "[Abp File Type]_Calling.template".
        /// </summary>
        Calling,

        /// <summary>
        /// Sometimes, Calling use Called to use core element undirectly.
        /// Calling template's name is "[Abp File Type]_Called.template".
        /// </summary>
        Called,

        /// <summary>
        /// Namespace list for importing statements.
        /// Using template's name is "[Abp File Type]_Using.template".
        /// </summary>
        Using,

        /// <summary>
        /// Element used to group variables using.
        /// Replace template's name is "[Abp File Type]_Group.template".
        /// </summary>
        Group
    }
}
