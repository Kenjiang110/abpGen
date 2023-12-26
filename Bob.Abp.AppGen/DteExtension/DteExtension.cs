using Bob.Abp.AppGen.Models;
using Bob.Abp.AppGen.Templates;
using EnvDTE;
//using Microsoft.VisualStudio.Shell.Interop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Bob.Abp.AppGen.DteExtension
{
    [SuppressMessage("Ussage", "VSTHRD010")]
    public static class DteExtension
    {
        #region Project

        /// <summary>
        /// Recursively find projects from solution folder
        /// </summary>
        private static IEnumerable<Project> GetProjects(Project slnFold)
        {
            foreach (ProjectItem item in slnFold.ProjectItems) //items in solution folder
            {
                Project subProject = item.SubProject; //solution folder's sub item must be a project and should use SubProject to access it.
                if (subProject != null)
                {
                    if (subProject.Kind == Constants.vsProjectKindSolutionItems) //solution folder project
                    {
                        foreach (var prj in GetProjects(subProject))  //recursively get projects from this folder.
                            yield return prj;
                    }
                    else if (subProject.Kind != Constants.vsProjectKindMisc) //not a folder or misc project, must be a true project
                    {
                        yield return subProject;
                    }
                }
            }
        }

        /// <summary>
        /// Get all projects in the solution.
        /// </summary>
        /// <param name="solution">solution</param>
        /// <returns>All projects</returns>
        public static List<Project> GetProjects(this Solution solution)
        {
            var projectList = new List<Project>();
            var dteProjects = solution.Projects.OfType<Project>(); //all project or solution folder
            foreach (var project in dteProjects)
            {
                if (project.Kind == Constants.vsProjectKindSolutionItems)  //is solution folder
                {
                    projectList.AddRange(GetProjects(project));  //recursively get all projects in this solution folder
                }
                else if (project.Kind != Constants.vsProjectKindMisc) //not a folder or misc project, must be a true project
                {
                    projectList.Add(project);
                }
            }

            return projectList;
        }

        #endregion

        #region ProjectItem

        /// <summary>
        /// Get first item of all selected items.
        /// </summary>
        /// <returns>first selected item or null if none selected.</returns>
        public static ProjectItem GetSelectedItem(this SelectedItems selectedItems)
        {
            ProjectItem projectItem = null;
            foreach (SelectedItem selectedItem in selectedItems)
            {
                projectItem = selectedItem.ProjectItem;
                if (projectItem != null) break;
                {
                    return projectItem;
                }
            }
            return projectItem;
        }

        /// <summary>
        /// Get fileName file in items, if file doesn't exist then return null
        /// </summary>
        private static ProjectItem GetFileProjectItem(this ProjectItems items, string fileName)
        {
            foreach (ProjectItem item in items)
            {
                if ((item.Kind == Constants.vsProjectItemKindPhysicalFolder || item.Kind == Constants.vsProjectItemKindPhysicalFile)
                    && string.Compare(item.Name, fileName, true) == 0)
                {
                    return item;
                }
            }
            return null;
        }

        public static ProjectItem GetFileProjectItem(this Project project, string fileName)
        {
            var item = GetFileProjectItem(project.ProjectItems, fileName);
            return item;
        }

        public static ProjectItem GetFileProjectItem(this ProjectItem projectItem, string fileName)
        {
            var item = GetFileProjectItem(projectItem.ProjectItems, fileName);
            return item;
        }

        /// <summary>
        /// Get path specified folder in project, if folders on path doesn't exist then create them.
        /// </summary>
        public static ProjectItem GetOrCreatePath(this Project project, string relativePath)
        {
            string[] pathSegments = relativePath?.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            ProjectItem projectItem = null;
            if (!pathSegments.IsEmpty())
            {
                string folderName = pathSegments[0];
                projectItem = project.GetFileProjectItem(folderName);
                if (projectItem == null && !folderName.ExistsInSameFolder(project.FileName, true))
                {
                    projectItem = project.ProjectItems.AddFolder(folderName);
                }

                for (int i = 1; i < pathSegments.Length; i++)
                {
                    folderName = pathSegments[i];
                    var folderItem = projectItem.GetFileProjectItem(folderName);
                    if (folderItem == null && !folderName.ExistsInTheFolder(projectItem.FileNames[0], true))
                    {
                        projectItem = projectItem.ProjectItems.AddFolder(folderName);
                    }
                    else
                    {
                        projectItem = folderItem;
                    }
                }
            }

            return projectItem;
        }

        #region Information about projectItem

        /// <summary>
        /// Get project itme's relative path to project where the item belonging. 
        /// </summary>
        /// <param name="projectItem"></param>
        /// <returns>do not end with directory sperator.</returns>
        public static (string, string) GetProjectPathAndRelativePath(this ProjectItem projectItem)
        {
            string prjPath = Path.GetDirectoryName(projectItem.ContainingProject.FileName);
            //sometimes, there are more than 1 files associating with a project item.
            return (prjPath, Utils.GetRelativePath(projectItem.FileNames[0], prjPath));
        }

        /// <summary>
        /// Search file names in item's fileNames according to specified ext name.
        /// </summary>
        /// <returns>full path file</returns>
        public static string GetFileNameByExt(this ProjectItem item, string ext = null)
        {
            string extFileName = null;
            for (short i = 0; i < item.FileCount; i++)
            {
                var fileName = item.FileNames[i];
                if (fileName.EndsWith(ext))
                {
                    extFileName = fileName;
                    break;
                }
            }
            return extFileName;
        }

        #endregion

        #region projectItem's file

        /// <summary>
        /// Try to backup the projectItem's file if exists.
        /// </summary>
        /// <param name="projectItem"></param>
        public static void BackupFile(this ProjectItem projectItem)
        {
            for (short i = 0; i < projectItem.FileCount; i++)
            {
                var targetFileName = projectItem.FileNames[i];
                Utils.BackupFile(targetFileName);
            }
        }

        /// <summary>
        /// Add srcFullFileName file to targetFolderPath folder in project. If file already exists, then rename it to bak file.
        /// </summary>
        public static ProjectItem AddFromFile(this Project project, string targetFolderRelativePath, string srcFullFileName, bool autoBackup = false)
        {
            string targetFileName = Path.GetFileName(srcFullFileName);
            //try to find target file projectItem
            ProjectItem targetFolderItem = project.GetOrCreatePath(targetFolderRelativePath);
            ProjectItems targetProjectItems = targetFolderItem == null ? project.ProjectItems : targetFolderItem.ProjectItems;
            ProjectItem targetFileItem = targetProjectItems.GetFileProjectItem(targetFileName);

            //if already exists 
            if (targetFileItem != null)
            {
                if (autoBackup)
                {
                    targetFileItem.BackupFile();
                }
                //File.Delete(targetFileItem.FileNames[0]);
                targetFileItem.Delete();
            }
            return targetProjectItems.AddFromFileCopy(srcFullFileName);
        }

        /// <summary>
        /// Add a file with content to project in the sepcified path.
        /// </summary>
        /// <param name="project">target project</param>
        /// <param name="content">file content</param>
        /// <param name="targetFolderRelativePath">file relative path</param>
        /// <param name="fileName">file name</param>
        public static ProjectItem AddFileFromContent(this Project project, string targetFolderRelativePath, string fileName, string content, bool autoBackup = false)
        {
            string file = Utils.CreateTempFile(fileName, content);
            try
            {
                return project.AddFromFile(targetFolderRelativePath, file, autoBackup);
            }
            finally
            {
                File.Delete(file);
            }
        }

        #endregion

        #endregion

        #region CodeElement

        /// <summary>
        /// Get all specified kinds elements from codeElements recursively.
        /// </summary>
        /// <param name="kinds">Every kind in kinds is wanted.</param>
        /// <param name="codeElements"></param>
        private static IEnumerable<CodeElement> GetAllCodeElements(CodeElements codeElements, params vsCMElement[] kinds)
        {
            foreach (CodeElement element in codeElements)
            {
                if (kinds.IsEmpty() || kinds.Contains(element.Kind))
                {
                    yield return element;
                }
                else
                {
                    foreach (CodeElement childElement in GetAllCodeElements(element.Children, kinds))
                    {
                        yield return childElement;
                    }
                }
            }
        }

        /// <summary>
        /// Get all specified kind elements from project item code model recursively.
        /// </summary>
        /// <param name="projectItem">Code model in this source file project item.</param>
        private static IEnumerable<CodeElement> GetAllCodeElements(this ProjectItem projectItem, params vsCMElement[] kinds)
        {
            foreach (CodeElement element in GetAllCodeElements(projectItem.FileCodeModel.CodeElements, kinds))
            {
                yield return element;
            }
        }

        /// <summary>
        /// Get the code element from codeElements recursively.
        /// </summary>
        /// <param name="codeElements">code elements.</param>
        /// <param name="name">if this kind of element doesn't have a name then this must be null, otherwise an exception will be thrown.</param>
        /// <returns>first element meet the kind and name or null</returns>
        private static CodeElement GetCodeElement(this CodeElements codeElements, string name, params vsCMElement[] kinds)
        {
            CodeElement codeElement = null;
            foreach (CodeElement element in GetAllCodeElements(codeElements, kinds))
            {
                if (string.IsNullOrEmpty(name) || element.Name == name)
                {
                    codeElement = element;
                    break;
                }
            }

            return codeElement;
        }

        /// <summary>
        /// Get the code element from project item recursively.
        /// </summary>
        /// <param name="projectItem">the project item.</param>
        /// <param name="name">if this kind of element doesn't have a name then this must be null, otherwise an exception will be thrown.</param>
        /// <returns>first element meet the kind and name or null</returns>
        public static CodeElement GetCodeElement(this ProjectItem projectItem, string name, params vsCMElement[] kinds)
        {
            return GetCodeElement(projectItem.FileCodeModel.CodeElements, name, kinds);
        }

        /// <summary>
        /// Is this codeClass is an entity.
        /// </summary>
        private static EntityKinds GetEntityKinds(this CodeClass codeClass)
        {
            if (codeClass != null)
            {
                foreach (CodeElement elem in codeClass.Bases)
                {
                    if (elem.Name == "AggregateRoot")
                    {
                        return EntityKinds.Entity | EntityKinds.Extensible;
                    }
                    else if (elem.Name == "Entity")
                    {
                        return EntityKinds.Entity;
                    }
                    return GetEntityKinds(elem as CodeClass);  //recursively
                }
            }

            return EntityKinds.None;
        }

        /// <summary>
        /// Try to get a entity class from projectItem.
        /// </summary>
        /// <param name="projectItem">Class deine project item.</param>
        /// <returns>CodeClass or null.</returns>
        public static CodeClass GetEntityClass(this ProjectItem projectItem, out EntityKinds entityKind)
        {
            CodeClass codeClass = null;
            entityKind = EntityKinds.None;
            foreach (CodeClass cc in projectItem.GetAllCodeElements(vsCMElement.vsCMElementClass))
            {
                string classList = string.Empty;
                entityKind = cc.GetEntityKinds();
                if (entityKind.HasFlag(EntityKinds.Entity))
                {
                    codeClass = cc;
                    break;
                }
            }

            return codeClass;
        }

        private static bool IsEnum(this CodeTypeRef propertyType)
        {
            return propertyType.TypeKind == vsCMTypeRef.vsCMTypeRefCodeType
                && propertyType.CodeType.Kind == vsCMElement.vsCMElementEnum;
        }

        public static string GetTypeNameFromSourceCode(this CodeProperty property)
        {
            var src = property.StartPoint.CreateEditPoint().GetText(property.EndPoint);
            //get typename from source code
            var segments = src.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var setgetIdx = Array.FindIndex(segments, s => s.Trim().StartsWith("{"));

            return setgetIdx > 1 ? segments[setgetIdx - 2] : null;
        }

        /// <summary>
        /// Get Enum type's members.
        /// </summary>
        /// <param name="codeType"></param>
        /// <returns>if not an enum type, return empty list.</returns>
        private static List<string> GetEnumMembers(CodeType codeType)
        {
            List<string> members = new List<string>();

            if (codeType is CodeEnum codeEnum)
            {
                foreach (CodeElement codeElement in codeEnum.Members)
                {
                    if (codeElement.Kind == vsCMElement.vsCMElementVariable)
                    {
                        members.Add(codeElement.Name);
                    }
                }
            }

            return members;
        }

        /// <summary>
        /// If the namespace "ns" has been imported in the projectItem?
        /// </summary>
        private static bool IsImported(this ProjectItem projectItem, string ns)
        {
            bool isImported = false;
            foreach (CodeElement codeElement in projectItem.FileCodeModel.CodeElements)
            {
                if (codeElement.Kind == vsCMElement.vsCMElementImportStmt)
                {
                    EnvDTE80.CodeImport codeImport = (EnvDTE80.CodeImport)codeElement;
                    if (codeImport.Namespace == ns)
                    {
                        isImported = true;
                        break;
                    }
                }
            }

            return isImported;
        }

        #endregion

        #region AbpHelper Models

        private static void ExtractPropertyInfo(this CodeElement codeElement, AhEntity entityModel)
        {
            if (codeElement.Kind == vsCMElement.vsCMElementProperty)
            {
                CodeProperty property = codeElement as CodeProperty;
                //Property Name
                EntityProperty entityProperty = new EntityProperty
                {
                    PropertyName = property.Name,
                    PublicSetter = property.Setter.Access == vsCMAccess.vsCMAccessPublic
                };
                //property type
                var propertyType = property.Type;
                var typeString = propertyType.AsString;
                var nsList = typeString.ExtractNamespace(out string typeName);
                typeString = property.GetTypeNameFromSourceCode();
                if (typeString != null)
                {
                    nsList.AddRange(typeString.ExtractNamespace(out typeName));  //remove all namespace prefixies.
                }
                foreach (var ns in nsList)
                {
                    if (ns != entityModel.Namespace && entityModel.ExtraUsings.IndexOf(ns) == -1)
                    {
                        entityModel.ExtraUsings.Add(ns);
                    }
                }
                entityProperty.PropertyType = typeName.TrimEnd('?');
                //Is Enum? Is List?
                entityProperty.IsEnum = propertyType.IsEnum();
                if (entityProperty.IsEnum)
                {
                    entityProperty.Members = GetEnumMembers(propertyType.CodeType);
                }
                else
                {
                    entityProperty.IsList = typeName.IsList(); //List
                    if (entityProperty.PropertyName == "ParentId")
                    {
                        entityModel.EntityKind |= EntityKinds.Hierarcy;
                    }
                    else if (entityProperty.PropertyName == "TenantId")
                    {
                        entityModel.EntityKind |= EntityKinds.MultiTenant;
                    }
                }
                //Nullable
                entityProperty.Nullable = typeName.EndsWith("?");
                //save property
                entityModel.AddProperty(entityProperty);
            }
        }

        private static IEnumerable<CodeClass> GetAllNotEntityAncestor(this CodeClass codeClass)
        {
            if (codeClass != null)
            {
                yield return codeClass;

                foreach (CodeClass bClass in codeClass.Bases)
                {
                    if (bClass.Name != "AggregateRoot" && bClass.Name != "Entity")
                    {
                        foreach (var aClass in GetAllNotEntityAncestor(bClass))
                        {
                            yield return aClass;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get Entity model from codeClass and saved file.
        /// </summary>
        /// <param name="codeClass">Entity CodeClass.</param>
        private static AhEntity FillEntityModel(this CodeClass codeClass, AhEntity entityModel)
        {
            //Get basic information
            foreach (var aClass in codeClass.GetAllNotEntityAncestor())
            {
                foreach (CodeElement codeElement in aClass.Members)
                {
                    codeElement.ExtractPropertyInfo(entityModel);
                }
            }

            //initial extra information
            entityModel.SetDefaultLanguageResource("en");
            entityModel.SetDefaultLanguageResource("zh-Hans", "新建{0}");

            //try to merge saved extra information
            if (File.Exists(entityModel.FullFileName))
            {
                var savedContent = File.ReadAllText(entityModel.FullFileName, Encoding.UTF8);
                entityModel.SavedEntity = JsonConvert.DeserializeObject<AhEntity>(savedContent);
            }

            return entityModel;
        }

        /// <summary>
        /// Build entity model (without solution information) from selected entities.
        /// </summary>
        /// <param name="entityPrjItem">The entity file projectItem.</param>
        /// <returns>Full information about Abp Solution and selected Entities.</returns>
        /// <exception cref="ApplicationException">If the selected file is not a file in the Domain project.</exception>
        public static AhEntity ToEntityModel(this ProjectItem entityPrjItem)
        {
            CodeClass entityCodeClass = null;
            EntityKinds entityKind = EntityKinds.None;
            //1.Check if it is an entity in domain project
            var domainPrj = entityPrjItem.ContainingProject;
            string shortDomainPrjName = AbpProjectType.Domain.GetShortProjectName();
            if (domainPrj != null && domainPrj.Name.EndsWith(shortDomainPrjName) && entityPrjItem.FileNames[0].EndsWith(".cs"))
            {
                entityCodeClass = entityPrjItem.GetEntityClass(out entityKind);
            }
            if (entityCodeClass == null || entityKind == EntityKinds.None) //and not an entity class code file
            {
                throw new ApplicationException("Please select an entity class file in the domain module project.");
            }

            //2.Build ahEntity model from entity projectItem
            (var prjPath, var relativePath) = entityPrjItem.GetProjectPathAndRelativePath();
            AhEntity entityModel = new AhEntity(entityKind, prjPath, relativePath, entityCodeClass.Name, entityCodeClass.Namespace.Name);
            entityModel.SetDefaultSkipSettings();
            entityCodeClass.FillEntityModel(entityModel);

            return entityModel;
        }

        /// <summary>
        /// Get Entity model from json Info file.
        /// </summary>
        /// <param name="entityPrjItem">json info file project item.</param>
        /// <param name="modelOnly">Without solution?</param>
        /// <returns>Full information about Abp Solution and selected Entities.</returns>
        /// <exception cref="ApplicationException">If the selected file is not a json Info file.</exception>
        public static AhEntity GetEntityModel(this ProjectItem entityPrjItem, bool modelOnly = false)
        {
            var domainPrj = entityPrjItem.ContainingProject;
            string shortDomainPrjName = AbpProjectType.Domain.GetShortProjectName();

            string fullFileName = entityPrjItem.FileNames[0];
            string jsonFullFileName = null;
            if (fullFileName.EndsWith(".cs"))
            {
                jsonFullFileName = fullFileName + ".json";
            }
            else if (fullFileName.EndsWith(".json"))
            {
                jsonFullFileName = fullFileName;
            }

            AhEntity entityModel;
            if (File.Exists(jsonFullFileName))
            {
                string content = File.ReadAllText(jsonFullFileName, Encoding.UTF8);
                entityModel = JsonConvert.DeserializeObject<AhEntity>(content)
                    ?? throw new ApplicationException("Please select an entity class or json info file in the domain module project.");
                entityModel.FullFileName = jsonFullFileName;
            }
            else
            {
                entityModel = entityPrjItem.ToEntityModel();
            }

            if (!modelOnly)
            {
                //3.Parse domain project's name to get root namespace and module's name
                var nsPrefix = domainPrj.Name.TrimEnd(shortDomainPrjName).TrimEnd('.');
                var rootNamespace = nsPrefix.ExtractNamespace(out string moduleName)[0];  //0 must exist .

                //4.Find all abp module projects.
                AhSolution abpSolution = new AhSolution(rootNamespace, moduleName);
                var prjList = new List<Project>(GetProjects(domainPrj.ParentProjectItem.ContainingProject));  //only search same folder as domain project.
                foreach (var aptValue in Enum.GetValues(typeof(AbpProjectType)))
                {
                    AbpProjectType apType = (AbpProjectType)aptValue;
                    string fullPrjName = apType.GetFullProjectName(nsPrefix, null);
                    string fullRootPrjName = apType.GetFullProjectName(nsPrefix, moduleName);
                    var project = prjList.FirstOrDefault(t => t.Name == fullPrjName || t.Name == fullRootPrjName);
                    abpSolution.SetProject(apType, project, project?.FileName);
                }

                entityModel.Solution = abpSolution;
            }
            return entityModel;
        }

        /// <summary>
        /// Convert AH project item to DTE project item.
        /// </summary>
        /// <param name="solution">abp solution full information object.</param>
        /// <returns>The AH project item specified dte projectItem. Null if not found.</returns>
        public static ProjectItem ToProjectItem(this AhEditProjectItem ahPrjItem, AhSolution solution)
        {
            ProjectItem projectItem;

            var prj = solution.GetProject(ahPrjItem.ProjectType) ?? throw new ApplicationException($"Could not find the {ahPrjItem.ProjectType} project.");
            if (!string.IsNullOrEmpty(ahPrjItem.RelativeFolder))
            {
                ProjectItem folder = prj.GetOrCreatePath(ahPrjItem.RelativeFolder);
                projectItem = folder.GetFileProjectItem(ahPrjItem.FileName);
            }
            else
            {
                projectItem = prj.GetFileProjectItem(ahPrjItem.FileName);
            }

            //if (projectItem == null)
            //{
            //    throw new ApplicationException($"There is no {ahPrjItem.RelativeFolder}{Path.DirectorySeparatorChar}{ahPrjItem.FileName} in {ahPrjItem.ProjectType} project.");
            //}
            return projectItem;
        }

        /// <summary>
        /// Find AH code element from the project item or from main element in the project item. 
        /// </summary>
        /// <param name="projectItem">The ahCodeElement.</param>
        /// <param name="mainElement">if mainElement is not null then use it directly else find it from project item.</param>
        /// <param name="removeIt">If this is a sub element then try remove it.</param>
        /// <returns>The code element if found or null.</returns>
        private static CodeElement ToCodeElement(this AhCodeElement ahCodeElement, ProjectItem projectItem, ref CodeElement mainElement, bool removeIt = false)
        {
            CodeElement codeElement;
            if (ahCodeElement.IsRootElement)
            {
                codeElement = projectItem.GetCodeElement(ahCodeElement.Name, ahCodeElement.Kind);
            }
            else
            {
                if (mainElement == null)
                {
                    mainElement = projectItem.GetCodeElement(null, vsCMElement.vsCMElementClass, vsCMElement.vsCMElementInterface);
                    if (mainElement == null)
                    {
                        throw new ApplicationException($"Coundn't find main element in project item {projectItem.Name}");
                    }
                }

                string eleName = ahCodeElement.GetRealName(mainElement.Name);
                codeElement = GetCodeElement(mainElement.Children, eleName, ahCodeElement.Kind);
                if (removeIt && codeElement != null)
                {
                    if (mainElement.Kind == vsCMElement.vsCMElementClass)
                    {
                        (mainElement as CodeClass).RemoveMember(codeElement);
                    }
                    else
                    {
                        (mainElement as CodeInterface).RemoveMember(codeElement);
                    }
                }
            }

            return codeElement;
        }

        /// <summary>
        /// Does AH code element from the project item or from main element exist?
        /// </summary>
        /// <param name="projectItem">The ahCodeElement.</param>
        /// <param name="mainElement">if mainElement is not null then use it directly else find it from project item.</param>
        /// <param name="removeIt">If this is a sub element then try remove it.</param>
        /// <returns>true if exists and false if don't.</returns>
        public static bool CodeElementExists(this AhCodeElement ahCodeElement, ProjectItem projectItem, ref CodeElement mainElement, bool removeIt)
        {
            CodeElement codeElement = ahCodeElement.ToCodeElement(projectItem, ref mainElement, removeIt);
            return codeElement != null;
        }

        /// <summary>
        /// Convert AH edit point to DTE edit point in project item or class.
        /// </summary>
        /// <param name="mainElement">if main element is not null then try to use it directly while needed.</param>
        /// <param name="ahEp">Information about the edit point in project item.</param>
        /// <param name="projectItem"></param>
        /// <returns>null if not ingoreIfExists = true and does exist already.</returns>
        public static EditPoint ToEditPoint(this AhEditPoint ahEp, ProjectItem projectItem, ref CodeElement mainElement)
        {
            Positions pos = ahEp.Position;
            CodeElement codeElement = ahEp.ToCodeElement(projectItem, ref mainElement);
            if (codeElement == null)
            {
                if (codeElement == null && ahEp.FallBackToRoot)
                {
                    pos = Positions.End;
                    var fbEp = new AhEditPoint(null, vsCMElement.vsCMElementClass, pos, ahEp.TemplateType);
                    codeElement = fbEp.ToCodeElement(projectItem, ref mainElement);
                }
                if (codeElement == null)
                {
                    throw new ApplicationException($"Coundn't find {ahEp.Kind} : {ahEp.Name} in project item {projectItem.Name}");
                }
            }

            EditPoint ep;
            if (pos.HasFlag(Positions.End)) //end of the element
            {
                //not ask for after end && has body
                if (!pos.HasFlag(Positions.BeforeOrAfter) && codeElement.Kind.HasBody())
                {
                    //ask for end of the body. 
                    ep = codeElement.GetEndPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
                }
                else
                {
                    //ask for end of the whole part.
                    ep = codeElement.GetEndPoint().CreateEditPoint();
                }
            }
            else //must is Positions.Start
            {
                //not ask for before start && has body
                if (!pos.HasFlag(Positions.BeforeOrAfter) && codeElement.Kind.HasBody())
                {
                    //ask for start of the body. 
                    ep = codeElement.GetStartPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
                }
                else
                {
                    //ask for start of the whole part.
                    ep = codeElement.GetStartPoint().CreateEditPoint();
                    ep.StartOfLine();
                }
            }
            if (pos.HasFlag(Positions.ExtraMove)) //extra movement
            {
                var ppos = pos ^ Positions.ExtraMove;
                //body start or body after
                if (ppos == Positions.Start || ppos == Positions.After)
                {
                    ep.LineDown();
                }
                else //header before or body end
                {
                    ep.LineUp();
                }
                if (ppos == Positions.After)
                {
                    ep.EndOfLine();
                }
                else if (ppos == Positions.Before)
                {
                    ep.StartOfLine();
                }
            }

            return ep;
        }

        /// <summary>
        /// Add uisng statements in projectItem file at edit point.
        /// </summary>
        public static void AddUsings(this ProjectItem projectItem, EditPoint editPoint, params string[] extraUsings)
        {
            //move to good position
            if (editPoint.Line == 1)
            {
                editPoint.Insert(Environment.NewLine);
            }
            editPoint.LineUp();

            //using extraUsings
            foreach (var us in extraUsings)
            {
                if (!projectItem.IsImported(us))
                {
                    editPoint.Insert($"using {us};{Environment.NewLine}");
                }
            }
        }

        /// <summary>
        /// Add variable define statements in code element at edit point.
        /// </summary>
        /// <param name="variables">variable in variables has three parts : prefix = varaibleName = value.</param>
        /// <param name="defaultEp">Default edit point used when not last variable.</param>
        /// <param name="codeElement">variables really added (not exist and not updated).</param>
        /// <returns>Actually added variable names.</returns>
        public static List<SimpleChainNode> AddVariables(this CodeElement codeElement, EditPoint defaultEp, params string[] variables)
        {
            List<SimpleChainNode> addedVaraibles = new List<SimpleChainNode>();
            string lastVarName = null;
            EditPoint editPoint = defaultEp;
            foreach (var variable in variables)
            {
                if (ParseVariableString(variable, out string varPrefix, out string varName, out string varValue))
                {
                    var varCodeElement = GetCodeElement(codeElement.Children, varName, vsCMElement.vsCMElementVariable);
                    if (varCodeElement != null)  //next variable should insert after this element.
                    {
                        editPoint = varCodeElement.GetStartPoint().CreateEditPoint();
                        editPoint.LineDown();
                        editPoint.StartOfLine();
                    }
                    else
                    {
                        addedVaraibles.AddNode(lastVarName, varName);

                        if (!string.IsNullOrEmpty(varValue))
                        {
                            varValue = " = " + varValue;
                        }
                        editPoint.Insert($"{varPrefix} {varName}{varValue};{Environment.NewLine}");
                    }
                    lastVarName = varName;
                }
                else
                {
                    editPoint.Insert($"{varValue}{Environment.NewLine}");
                }
            }

            return addedVaraibles;
        }

        private static bool ParseVariableString(this string variable, out string varPrefix, out string varName, out string varValue)
        {
            var varParts = variable?.Split('='); //prefix = varaibleName = value
            if (!varParts.IsEmpty(3))
            {
                (varPrefix, varName, varValue) = (varParts[0].TrimEnd(), varParts[1].Trim(), varParts[2].Trim());
                return true;
            }
            else
            {
                (varPrefix, varName, varValue) = (null, null, variable);
                return false;
            }
        }

        #endregion

        #region UI

        public static OutputWindowPane GetOrCreateOutputPane(this EnvDTE80.DTE2 dte, string paneName)
        {
            OutputWindowPane pane = null;
            foreach (OutputWindowPane pn in dte.ToolWindows.OutputWindow.OutputWindowPanes)
            {
                if (pn.Name == paneName)
                {
                    pane = pn;
                }
            }
            if (pane == null) pane = dte.ToolWindows.OutputWindow.OutputWindowPanes.Add(paneName);
            pane.Activate();

            return pane;
        }

        #endregion
    }
}
