using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bob.Abp.AppGen.DteExtension;
using System.Diagnostics.CodeAnalysis;
using EnvDTE80;
using System.IO;
using Bob.Abp.AppGen.Models;
using Bob.Abp.AppGen.Templates;
using Microsoft.VisualStudio.GraphModel.CodeSchema;

namespace Bob.Abp.AppGen.DteExtension
{
    [SuppressMessage("Ussage", "VSTHRD010")]
    public class AbpCodeGenerator
    {
        protected readonly AhSolution _sln;
        protected readonly AhEntity _entity;
        protected readonly bool _safeMode;

        public AbpCodeGenerator(AhEntity entity, bool safeMode)
        {
            _sln = entity.Solution;
            _entity = entity;
            _safeMode = safeMode;
        }

        public bool CreateOrUpdateLocalization()
        {
            var relativePath = $"Localization{Path.DirectorySeparatorChar}{_sln.ModuleName}";
            string fullfolderPathName = _sln.GetAbsolutePath(AbpProjectType.Shared, relativePath);

            bool done = false;
            foreach (var language in _entity.Languages)
            {
                LocalizationFile lf = new LocalizationFile(fullfolderPathName, language.Culture, !_safeMode);
                lf.Add($"Menu:{_entity.Name}", language.DisplayName, true);
                lf.Add($"New{_entity.Name}", language.NewEntityCmd);
                lf.Add(language.Properties, _entity.Name);
                lf.Add(language.Enums);

                lf.Save(false);
                done = true;
            }

            return done;
        }

        public void CreateOrEditFiles(AbpMainFile abpFile)
        {
            var ahPrjItems = abpFile.GetAhProjectItems(_entity);
            var templateData = new TemplateData(_entity);
            foreach (var ahProjectItem in ahPrjItems)
            {
                if (ahProjectItem is AhEditProjectItem ahPrjItem)
                {
                    EditFile(templateData, ahPrjItem);
                }
                else
                {
                    var prj = _sln.GetProject(ahProjectItem.ProjectType);
                    string fileName = ahProjectItem.FileName;
                    string fullFileName = _sln.GetAbsolutePath(ahProjectItem.ProjectType, ahProjectItem.RelativeFolder, fileName);
                    if ((!ahProjectItem.Secured && !_safeMode) || !File.Exists(fullFileName))
                    {
                        TemplateData data = new TemplateData(_entity);
                        string codes = data.Build(ahProjectItem);
                        prj.AddFileFromContent(ahProjectItem.RelativeFolder, fileName, codes);
                    }
                }
            }
        }

        private bool EditFile(TemplateData templateData, AhEditProjectItem ahPrjItem)
        {
            var prjItem = ahPrjItem.ToProjectItem(_sln);
            if (prjItem == null) return false;  //ignore if file to modify donesn't exist.

            CodeElement mainCodeElement = null;
            //Core element's existence means assistant element was already prepared (added).
            //And !safeMode means force to update the core element and only the core element.
            bool exists = ahPrjItem.CoreElement.CodeElementExists(prjItem, ref mainCodeElement, removeIt: !_safeMode && !ahPrjItem.Secured);
            if (exists && (ahPrjItem.Secured || _safeMode))
            {
                return false;
            }

            foreach (var ahEp in ahPrjItem.EditPoints)
            {
                EditPoint editPoint = ahEp.ToEditPoint(prjItem, ref mainCodeElement);

                if (ahPrjItem.CoreElement.Kind == vsCMElement.vsCMElementVariable
                    && ahEp.Is(TemplateType.Main))
                {
                    var content = templateData.Build(ahEp);
                    var vars = content.ToLines(StringSplitOptions.None);
                    templateData.VariableChain = mainCodeElement.AddVariables(editPoint, vars);  //avoid duplicately add middle variables
                }
                else if (ahEp.Is(TemplateType.Using) && !exists)
                {
                    var usNamespaces = templateData.BuildUsing(ahEp);
                    prjItem.AddUsings(editPoint, usNamespaces);
                }
                //if Main tempalte then core element must has been removed or core element never added.
                else if (ahEp.Is(TemplateType.Main) || !exists)
                {
                    var content = templateData.Build(ahEp);
                    editPoint.Insert(content);
                }
            }

            if (prjItem.IsDirty) prjItem.Save();
            return true;
        }
    }
}
