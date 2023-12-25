using EnvDTE;
using Bob.Abp.AppGen.DteExtension;
using Bob.Abp.AppGen.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bob.Abp.AppGen.Models
{
    /// <summary>
    /// Files to be edited.
    /// </summary>
    public enum AbpEditFile
    {
        Application_AutoMapper,

        Contracts_PermissionConst,
        Contracts_PermissionDefine,

        MongoDB_IDbContext,
        MongoDB_DbContext,
        EntityFrameworkCore_IDbContext,
        EntityFrameworkCore_DbContext,
        EntityFrameworkCore_CreatingExtensions,

        Web_AutoMapper,
        Web_PageToolbar,
        Web_PageAuthorization,

        Web_Menu_Consts,
        Web_Menu_ContributorModify,
        Web_Menu_Contributor
    }

    public static class AbpEditFileExtensions
    {
        /// <summary>
        /// {0} is moduleName, {1} is entityName
        /// </summary>
        private static readonly AhEditProjectItem[] ahProjectItems = new AhEditProjectItem[]
        {
            new AhEditProjectItem(AbpProjectType.Application, String.Empty, "{0}ApplicationAutoMapperProfile.cs", vsCMElement.vsCMElementFunction, "CreateMap{1}")
                .AddEditPoint(TemplateType.Using, null, vsCMElement.vsCMElementNamespace, Positions.Before)
                .AddEditPoint(TemplateType.Calling, null, vsCMElement.vsCMElementFunction, Positions.End)
                .AddEditPoint(TemplateType.Main, null, vsCMElement.vsCMElementClass, Positions.End),

            new AhEditProjectItem(AbpProjectType.Contracts, "Permissions", "{0}Permissions.cs", vsCMElement.vsCMElementClass, "{1}")
                .AddEditPoint(TemplateType.Main, "GetAll", vsCMElement.vsCMElementFunction, Positions.Before, true),
            new AhEditProjectItem(AbpProjectType.Contracts, "Permissions", "{0}PermissionDefinitionProvider.cs", vsCMElement.vsCMElementFunction, "Define{1}")
                .AddEditPoint(TemplateType.Calling, "Define", vsCMElement.vsCMElementFunction, Positions.End)
                .AddEditPoint( TemplateType.Main,"L", vsCMElement.vsCMElementFunction, Positions.Before),

            new AhEditProjectItem(AbpProjectType.MongoDB, "MongoDB", "I{0}MongoDbContext.cs", vsCMElement.vsCMElementProperty, "{1}_s")
                .AddEditPoint(TemplateType.Using, null, vsCMElement.vsCMElementNamespace, Positions.Before)
                .AddEditPoint(TemplateType.Main, null, vsCMElement.vsCMElementInterface, Positions.End),
            new AhEditProjectItem(AbpProjectType.MongoDB, "MongoDB", "{0}MongoDbContext.cs", vsCMElement.vsCMElementProperty, "{1}_s")
                .AddEditPoint(TemplateType.Using, null, vsCMElement.vsCMElementNamespace, Positions.Before)
                .AddEditPoint(TemplateType.Calling, "CreateModel", vsCMElement.vsCMElementFunction, Positions.End)
                .AddEditPoint(TemplateType.Main, "CreateModel", vsCMElement.vsCMElementFunction, Positions.Before),
            new AhEditProjectItem(AbpProjectType.EntityFrameworkCore, "EntityFrameworkCore", "I{0}DbContext.cs", vsCMElement.vsCMElementProperty, "{1}_s")
                .AddEditPoint(TemplateType.Using, null, vsCMElement.vsCMElementNamespace, Positions.Before)
                .AddEditPoint(TemplateType.Main, null, vsCMElement.vsCMElementInterface, Positions.End),
            new AhEditProjectItem(AbpProjectType.EntityFrameworkCore, "EntityFrameworkCore", "{0}DbContext.cs", vsCMElement.vsCMElementProperty, "{1}_s")
                .AddEditPoint(TemplateType.Using, null, vsCMElement.vsCMElementNamespace, Positions.Before)
                .AddEditPoint(TemplateType.Main, "{0}DbContext", vsCMElement.vsCMElementFunction, Positions.Before),
            new AhEditProjectItem(AbpProjectType.EntityFrameworkCore, "EntityFrameworkCore", "{0}DbContextModelCreatingExtensions.cs", vsCMElement.vsCMElementFunction, "Configure{1}")
                .AddEditPoint(TemplateType.Using, null, vsCMElement.vsCMElementNamespace, Positions.Before)
                .AddEditPoint(TemplateType.Calling, "Configure{0}", vsCMElement.vsCMElementFunction, Positions.End)
                .AddEditPoint(TemplateType.Main, null, vsCMElement.vsCMElementClass, Positions.End),

            new AhEditProjectItem(AbpProjectType.Web, String.Empty, "{0}WebAutoMapperProfile.cs", vsCMElement.vsCMElementFunction, "CreateMap{1}")
                .AddEditPoint(TemplateType.Using, null, vsCMElement.vsCMElementNamespace, Positions.Before)
                .AddEditPoint(TemplateType.Calling, null, vsCMElement.vsCMElementFunction, Positions.End)
                .AddEditPoint(TemplateType.Main, null, vsCMElement.vsCMElementClass, Positions.End),
            new AhEditProjectItem(AbpProjectType.Web, String.Empty, "{0}WebModule.cs", vsCMElement.vsCMElementFunction, "Configure{1}PageToolbar")
                .AddEditPoint(TemplateType.Using, null, vsCMElement.vsCMElementNamespace, Positions.Before)
                .AddEditPoint(TemplateType.Calling, "ConfigureServices", vsCMElement.vsCMElementFunction, Positions.End)
                .AddEditPoint(TemplateType.Main, "ConfigureServices", vsCMElement.vsCMElementFunction, Positions.After),
            new AhEditProjectItem(AbpProjectType.Web, String.Empty, "{0}WebModule.cs", vsCMElement.vsCMElementFunction, "Configure{1}PageAuthorization")
                .AddEditPoint(TemplateType.Using, null, vsCMElement.vsCMElementNamespace, Positions.Before)
                .AddEditPoint(TemplateType.Calling, "ConfigureServices", vsCMElement.vsCMElementFunction, Positions.End)
                .AddEditPoint(TemplateType.Main, "ConfigureServices", vsCMElement.vsCMElementFunction, Positions.After),

            new AhEditProjectItem(AbpProjectType.Web, "Menus", "{0}Menus.cs", vsCMElement.vsCMElementVariable, "{1}" )
                .AddEditPoint(TemplateType.Main, null, vsCMElement.vsCMElementClass, Positions.End),
            new AhEditProjectItem(AbpProjectType.Web, "Menus", "{0}MenuContributor.cs", vsCMElement.vsCMElementFunction, "Configure{0}MenuAsync", secured: true)
                .AddEditPoint(TemplateType.Using, null, vsCMElement.vsCMElementNamespace, Positions.Before)
                .AddEditPoint(TemplateType.Calling, "ConfigureMenuAsync", vsCMElement.vsCMElementFunction, Positions.End)
                .AddEditPoint(TemplateType.Main, null, vsCMElement.vsCMElementClass, Positions.End),
            new AhEditProjectItem(AbpProjectType.Web, "Menus", "{0}MenuContributor.cs", vsCMElement.vsCMElementVariable, "dn{1}")
                .AddEditPoint(TemplateType.Main, "Configure{0}MenuAsync", vsCMElement.vsCMElementFunction, Positions.Before)
                .AddEditPoint(TemplateType.Calling, "Configure{0}MenuAsync", vsCMElement.vsCMElementFunction, Positions.End | Positions.ExtraMove),
       };

        /// <summary>
        /// Get file's project item information about to be edited.
        /// </summary>
        /// <param name="sln"></param>
        /// <param name="abpFile"></param>
        /// <returns></returns>
        public static AhEditProjectItem GetAhProjectItem(this AbpEditFile abpFile, AhEntity entity)
        {
            int idx = (int)abpFile;
            var item = ahProjectItems[idx];

            item.SetRealNames(entity.Solution.ModuleName, entity.Name);

            return item;
        }
    }
}
