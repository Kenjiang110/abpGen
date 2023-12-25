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
    public class AhSolution
    {
        /// <summary>
        /// Cached projects
        /// </summary>
        public Dictionary<AbpProjectType, Project> _projects = new Dictionary<AbpProjectType, Project>();

        /// <summary>
        /// Cached projects' root path.
        /// </summary>
        public readonly Dictionary<AbpProjectType, string> _rootPathes = new Dictionary<AbpProjectType, string>();

        /// <summary>
        /// All Abp modules's namespace prefix
        /// Assume domain project's name is [RootNamespace].[ModuleName].Domain,
        /// so if project's name is "Bob.Heros.WatchTower.Domain" 
        /// then RootNamespace = "Bob.Heros" 
        /// </summary>
        public string RootNamespace { get; set; }

        /// <summary>
        /// The module's name
        /// Assume domain project's name is [RootNamespace].[ModuleName].Domain,
        /// so if project's name is "Bob.Heros.WatchTower.Domain" 
        /// then ModuleName = "WatchTower" 
        /// </summary>
        public string ModuleName { get; set; }

        public AhSolution(string rootNamespace, string moduleName)
        {
            this.RootNamespace = rootNamespace;
            this.ModuleName = moduleName;
        }

        public void SetProject(AbpProjectType apType, Project project, string projectFileName)
        {
            if (project != null)
            {
                _projects[apType] = project;
                _rootPathes[apType] = Path.GetDirectoryName(projectFileName);
            }
        }

        public Project GetProject(AbpProjectType apType)
        {
            _projects.TryGetValue(apType, out Project project);
            return project;
        }

        public int ProjectCount => _projects.Count;

        public string GetFullProjectName(AbpProjectType apType)
        {
            return $"{this.RootNamespace}.{this.ModuleName}.{apType.GetShortProjectName()}";
        }

        /// <summary>
        /// Convert relative path of apTypes project to absolute path.
        /// </summary>
        /// <param name="apTypes">project's type.</param>
        /// <param name="relativePath"></param>
        public string GetAbsolutePath(AbpProjectType apTypes, params string[] relativePath)
        {
            var path = _rootPathes[apTypes];
            foreach (var rp in relativePath)
            {
                path = Path.Combine(path, rp);
            }

            return path;
        }
    }
}
