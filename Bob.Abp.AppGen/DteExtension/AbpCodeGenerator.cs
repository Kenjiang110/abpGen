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
                lf.Add(language.Properties);
                lf.Add(language.Enums);

                lf.Save(false);
                done = true;
            }

            return done;
        }

        /// <summary>
        /// Create code file for abpGenFile: 
        ///     Contracts_EntityDto, Contracts_RequestDto, Contracts_CreateUpdateDto, Contracts_IAppService,
        ///     Application_AppService
        /// </summary>
        /// <param name="abpFile"></param>
        public bool CreateCodeFile(AbpCreateFile abpFile)
        {
            var ahPrjItem = abpFile.GetAhProjectItem(_entity);
            var prj = _sln.GetProject(ahPrjItem.ProjectType);
            string fileName = ahPrjItem.FileName;
            string fullFileName = _sln.GetAbsolutePath(ahPrjItem.ProjectType, ahPrjItem.RelativeFolder, fileName);
            if ((!ahPrjItem.Secured && !_safeMode) || !File.Exists(fullFileName))
            {
                TemplateData data = new TemplateData(_entity);
                string codes = data.Build(abpFile);
                var item = prj.AddFileFromContent(ahPrjItem.RelativeFolder, fileName, codes);
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Contracts_PermissionConst, Contracts_PermissionDefine
        /// </summary>
        /// <param name="abpFile"></param>
        public bool ModifyCodeFile(AbpEditFile abpFile)
        {
            var templateData = new TemplateData(_entity);
            var ahPrjItem = abpFile.GetAhProjectItem(_entity);
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
                    && ahEp.TemplateType == TemplateType.Main)
                {
                    var content = templateData.Build(abpFile, ahEp.TemplateType);
                    var vars = content.ToLines(StringSplitOptions.None);
                    templateData.VariableChain = mainCodeElement.AddVariables(editPoint, vars);  //avoid duplicately add middle variables
                }
                else
                {
                    if (ahEp.TemplateType == TemplateType.Using && !exists)
                    {
                        var usNamespaces = templateData.BuildUsing(abpFile);
                        prjItem.AddUsings(editPoint, usNamespaces);
                    }
                    //if Main tempalte then core element must has been removed or core element never added.
                    else if (ahEp.TemplateType == TemplateType.Main || !exists)
                    {
                        var content = templateData.Build(abpFile, ahEp.TemplateType);
                        editPoint.Insert(content);
                    }
                }
            }

            if (prjItem.IsDirty) prjItem.Save();
            return true;
        }
    }
}
