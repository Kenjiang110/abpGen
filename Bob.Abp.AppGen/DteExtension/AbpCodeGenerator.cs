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
            foreach (var ahPrjItem in ahPrjItems)
            {
                if (ahPrjItem is AhEditProjectItem)
                {
                    EditFile(templateData, ahPrjItem as AhEditProjectItem);
                }
                else
                {
                    CreateFile(ahPrjItem);
                }
            }
        }

        private bool CreateFile(AhProjectItem ahPrjItem)
        {
            var prj = _sln.GetProject(ahPrjItem.ProjectType);
            string fileName = ahPrjItem.FileName;
            string fullFileName = _sln.GetAbsolutePath(ahPrjItem.ProjectType, ahPrjItem.RelativeFolder, fileName);
            if ((!ahPrjItem.Secured && !_safeMode) || !File.Exists(fullFileName))
            {
                TemplateData data = new TemplateData(_entity);
                string codes = data.Build(ahPrjItem);
                if (codes != null)
                {
                    prj.AddFileFromContent(ahPrjItem.RelativeFolder, fileName, codes);
                    return true;
                }
            }
            return false;
        }

        private bool EditFile(TemplateData templateData, AhEditProjectItem ahPrjItem)
        {
            var prjItem = ahPrjItem.ToProjectItem(_sln);
            if (prjItem == null)
            {
                if (!CreateFile(ahPrjItem)) return false;

                prjItem = ahPrjItem.ToProjectItem(_sln);
                if (prjItem == null) return false;
            }
            CodeElement mainCodeElement = null;
            //Core element's existence means assistant element was already prepared (added).
            //And !safeMode means force to update the core element and only the core element.
            var removeIt = !_safeMode && !ahPrjItem.Secured;
            if (ahPrjItem.CoreElement.CodeElementExists(prjItem, ref mainCodeElement, removeIt))
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
                else if (ahEp.Is(TemplateType.Using))
                {
                    var usNamespaces = templateData.BuildUsing(ahEp);
                    prjItem.AddUsings(editPoint, usNamespaces);
                }
                else
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
