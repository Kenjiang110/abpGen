using EnvDTE;
using Bob.Abp.AppGen.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob.Abp.AppGen.Models
{
    /// <summary>
    /// A project item to be created or edited.
    /// </summary>
    public class AhProjectItem
    {
        /// <summary>
        /// The item's project's type.
        /// </summary>
        public AbpProjectType ProjectType { get; set; }

        /// <summary>
        /// The item's folder path relative to the project.
        /// </summary>
        public string RelativeFolder { get; set; }
        private string RelativeFolderTemplate { get; set; }
        private bool UsingUpFolder { get; set; }


        /// <summary>
        /// The item file's name.
        /// </summary>
        public string FileName { get; set; }
        private string FileNameTemplate { get; set; }

        public string TemplateName { get; set; }


        /// <summary>
        /// Secured means force into safe mode.
        /// </summary>
        public bool Secured { get; set; }

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public AhProjectItem()
        {
        }

        /// <summary>
        /// Contructor.
        /// </summary>
        public AhProjectItem(AbpProjectType projectType, string relativePath, string fileName, string templateName, bool secured = false)
        {
            const string upDirectoryFlag = "..";
            if (relativePath.EndsWith(upDirectoryFlag))
            {
                relativePath = relativePath.TrimEnd(upDirectoryFlag);
                UsingUpFolder = true;
            }
            else
            {
                UsingUpFolder = false;
            }
            RelativeFolderTemplate = relativePath;
            
            ProjectType = projectType;
            FileNameTemplate = fileName;
            TemplateName = $"{projectType}.{templateName}";
            Secured = secured;
        }

        /// <summary>
        /// Set file name and relative folder by mouduleName, entityName and relativePath.
        /// </summary>
        /// <param name="relativePath">entity relativePath for AhProjectItem only.</param>
        /// <returns>This object for chain calling.</returns>
        public virtual AhProjectItem SetRealNames(string moduleName, string entityName, string relativePath)
        {
            FileName = String.Format(FileNameTemplate, moduleName, entityName, relativePath);
            if (UsingUpFolder)
            {
                relativePath = relativePath.GetUpFolder();
            }
            RelativeFolder = string.Format(RelativeFolderTemplate, moduleName, entityName, relativePath, Path.DirectorySeparatorChar);

            return this;
        }
    }
}
