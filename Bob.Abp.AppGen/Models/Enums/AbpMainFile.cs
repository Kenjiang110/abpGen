using Bob.Abp.AppGen.Templates;
using EnvDTE;
using System;

namespace Bob.Abp.AppGen.Models
{
    public enum AbpMainFile
    {
        Contracts_Dtos,  //1
        Contracts_RemoteServiceConsts,  //2
        MongoDB_Repository,  //3
        EntityFrameworkCore_Repository,  //4
        Permission,  //5
        AppService,  //6
        Web_ViewModel,  //7
        HttpApi_Controller,  //8
        Web_Menu,  //9
        Web_Pages, //10
        Web_Page_ExtraJs,  //11
    }

    public static class AbpMainFileExtension
    {
        /// <summary>
        /// {0} is ModuleName
        /// {1} is EntityName for file name and folderPath,
        /// {2} is EntityRelativePath for folderPath , 
        /// {3} is Path.DirectorySeparatorChar
        /// </summary>
        private static readonly AhProjectItem[][] ahProjectItems = new AhProjectItem[][]
        {
            //1.Dtos
            new [] {
                new AhProjectItem(AbpProjectType.Contracts, "{2}{3}{1}", "{1}Dto.cs", "DtoEntity"),
                new AhProjectItem(AbpProjectType.Contracts, "{2}{3}{1}", "{1}RequestDto.cs", "DtoRequest"),
                new AhProjectItem(AbpProjectType.Contracts, "{2}{3}{1}", "{1}CreateOrUpdateDtoBase.cs", "DtoCreateUpdateBase"),
                new AhProjectItem(AbpProjectType.Contracts, "{2}{3}{1}", "{1}CreateDto.cs", "DtoCreate"),
                new AhProjectItem(AbpProjectType.Contracts, "{2}{3}{1}", "{1}UpdateDto.cs", "DtoUpdate")
            },

            //2.RemoteServiceConsts
            new [] { new AhProjectItem(AbpProjectType.Contracts, string.Empty, "{0}RemoteServiceConsts.cs", "RemoteServiceConsts", secured: true) },

            //3.MongoDB Repository
            new[] {
                new AhProjectItem(AbpProjectType.Shared, "{2}..", "{1}Consts.cs", "EntityConsts"),
                new AhProjectItem(AbpProjectType.Domain, "{2}", "I{1}Repository.cs", "IRepository"),
                new AhEditProjectItem(AbpProjectType.MongoDB, "MongoDB", "I{0}MongoDbContext.cs", vsCMElement.vsCMElementProperty, "{1}_s", "IDbContext")
                    .AddEditPoint(null, vsCMElement.vsCMElementNamespace, Positions.Before, TemplateType.Using)
                    .AddEditPoint(null, vsCMElement.vsCMElementInterface, Positions.End, TemplateType.Main),
                new AhEditProjectItem(AbpProjectType.MongoDB, "MongoDB", "{0}MongoDbContext.cs", vsCMElement.vsCMElementProperty, "{1}_s", "DbContext")
                    .AddEditPoint(null, vsCMElement.vsCMElementNamespace, Positions.Before, TemplateType.Using)
                    .AddEditPoint("CreateModel", vsCMElement.vsCMElementFunction, Positions.End, TemplateType.Calling)
                    .AddEditPoint("CreateModel", vsCMElement.vsCMElementFunction, Positions.Before, TemplateType.Main),
            },

            //4.EntityFrameworkCore Repository
            new[] {
                new AhProjectItem(AbpProjectType.Shared, "{2}..", "{1}Consts.cs", "EntityConsts"),
                new AhProjectItem(AbpProjectType.Domain, "{2}", "I{1}Repository.cs", "IRepository"),
                new AhProjectItem(AbpProjectType.EntityFrameworkCore, "{2}", "{1}Repository.cs", "Repository"),
                new AhEditProjectItem(AbpProjectType.EntityFrameworkCore, "EntityFrameworkCore", "I{0}DbContext.cs", vsCMElement.vsCMElementProperty, "{1}_s", "IDbContext")
                    .AddEditPoint(null, vsCMElement.vsCMElementNamespace, Positions.Before, TemplateType.Using)
                    .AddEditPoint(null, vsCMElement.vsCMElementInterface, Positions.End, TemplateType.Main),
                new AhEditProjectItem(AbpProjectType.EntityFrameworkCore, "EntityFrameworkCore", "{0}DbContext.cs", vsCMElement.vsCMElementProperty, "{1}_s", "DbContext")
                    .AddEditPoint(null, vsCMElement.vsCMElementNamespace, Positions.Before, TemplateType.Using)
                    .AddEditPoint("{0}DbContext", vsCMElement.vsCMElementFunction, Positions.Before, TemplateType.Main),
                new AhEditProjectItem(AbpProjectType.EntityFrameworkCore, "EntityFrameworkCore", "{0}DbContext.cs", vsCMElement.vsCMElementOther, "builder.Configure{0}()", "Configure", secured: true)
                    .AddEditPoint("OnModelCreating", vsCMElement.vsCMElementFunction, Positions.End, TemplateType.Main),
                new AhEditProjectItem(AbpProjectType.EntityFrameworkCore, "EntityFrameworkCore", "{0}DbContextModelCreatingExtensions.cs", vsCMElement.vsCMElementFunction, "Configure{1}", "CreatingExtensions")
                    .AddEditPoint(null, vsCMElement.vsCMElementNamespace, Positions.Before, TemplateType.Using)
                    .AddEditPoint("Configure{0}", vsCMElement.vsCMElementFunction, Positions.End, TemplateType.Calling)
                    .AddEditPoint(null, vsCMElement.vsCMElementClass, Positions.End, TemplateType.Main),
            },

            //5.Permission
            new [] {
                new AhEditProjectItem(AbpProjectType.Contracts, "Permissions", "{0}Permissions.cs", vsCMElement.vsCMElementClass, "{1}", "PermissionConst")
                    .AddEditPoint("GetAll", vsCMElement.vsCMElementFunction, Positions.Before, TemplateType.Main, true),
                new AhEditProjectItem(AbpProjectType.Contracts, "Permissions", "{0}PermissionDefinitionProvider.cs", vsCMElement.vsCMElementFunction, "Define{1}", "PermissionDefine")
                    .AddEditPoint("Define", vsCMElement.vsCMElementFunction, Positions.End, TemplateType.Calling)
                    .AddEditPoint("L", vsCMElement.vsCMElementFunction, Positions.Before, TemplateType.Main),
            },

            //6.AppService
            new[] {
                new AhProjectItem(AbpProjectType.Contracts, "{2}{3}{1}", "I{1}AppService.cs", "IAppService"),
                new AhEditProjectItem(AbpProjectType.Application, String.Empty, "{0}ApplicationAutoMapperProfile.cs", vsCMElement.vsCMElementFunction, "CreateMap{1}", "AutoMapper")
                    .AddEditPoint(null, vsCMElement.vsCMElementNamespace, Positions.Before, TemplateType.Using)
                    .AddEditPoint(null, vsCMElement.vsCMElementFunction, Positions.End, TemplateType.Calling)
                    .AddEditPoint(null, vsCMElement.vsCMElementClass, Positions.End, TemplateType.Main),
                new AhProjectItem(AbpProjectType.Application, "{2}..", "{1}AppService.cs", "AppService")
            },

            //7.ViewModel
            new[] {
                new AhEditProjectItem(AbpProjectType.Web, String.Empty, "{0}WebAutoMapperProfile.cs", vsCMElement.vsCMElementFunction, "CreateMap{1}","AutoMapper")
                    .AddEditPoint(null, vsCMElement.vsCMElementNamespace, Positions.Before, TemplateType.Using)
                    .AddEditPoint(null, vsCMElement.vsCMElementFunction, Positions.End, TemplateType.Calling)
                    .AddEditPoint(null, vsCMElement.vsCMElementClass, Positions.End, TemplateType.Main),
                new AhProjectItem(AbpProjectType.Web, "Pages{3}{2}{3}{1}", "{1}InfoModel.cs", "Page.ViewModel")
            },

            //8.HttpApi Controller
            new[] { new AhProjectItem(AbpProjectType.HttpApi, "{2}..", "{1}Controller.cs", "Controller") },

            //9.Menu
            new[] {
                new AhProjectItem(AbpProjectType.Web, "Menus", "MenuItemInfo.cs", "Menu.MenuItemInfo", secured: true),
                new AhEditProjectItem(AbpProjectType.Web, "Menus", "{0}MenuContributor.cs", vsCMElement.vsCMElementFunction, "Configure{0}MenuAsync", "Menu.ContributorModify", secured: true)
                    .AddEditPoint(null, vsCMElement.vsCMElementNamespace, Positions.Before, TemplateType.Using)
                    .AddEditPoint("ConfigureMenuAsync", vsCMElement.vsCMElementFunction, Positions.End | Positions.ExtraMove, TemplateType.Calling)
                    .AddEditPoint(null, vsCMElement.vsCMElementClass, Positions.End, TemplateType.Main),
                new AhEditProjectItem(AbpProjectType.Web, "Menus", "{0}Menus.cs", vsCMElement.vsCMElementVariable, "{1}", "Menu.Consts" )
                    .AddEditPoint(null, vsCMElement.vsCMElementClass, Positions.End, TemplateType.Main),
                new AhEditProjectItem(AbpProjectType.Web, "Menus", "{0}MenuContributor.cs", vsCMElement.vsCMElementVariable, "dn{1}", "Menu.Contributor")
                    .AddEditPoint("Configure{0}MenuAsync", vsCMElement.vsCMElementFunction, Positions.Before, TemplateType.Main)
                    .AddEditPoint("Configure{0}MenuAsync", vsCMElement.vsCMElementFunction, Positions.End | Positions.ExtraMove, TemplateType.Calling),
            },

            //10. Pages
            new[] {
                new AhEditProjectItem(AbpProjectType.Web, String.Empty, "{0}WebModuleConfigureExtensions.cs", vsCMElement.vsCMElementOther, "//Pages/{2}/{1}.Toolbar", "ConfigureExtensions", secured: true)
                    .AddEditPoint(null, vsCMElement.vsCMElementNamespace, Positions.Before, "Toolbar_" + TemplateType.Using)
                    .AddEditPoint("ConfigureToolbarOptions", vsCMElement.vsCMElementFunction, Positions.End, "Toolbar_" + TemplateType.Main),
                new AhEditProjectItem(AbpProjectType.Web, String.Empty, "{0}WebModuleConfigureExtensions.cs", vsCMElement.vsCMElementOther, "//Pages/{2}/{1}.Authorization", "ConfigureExtensions", secured: true)
                    .AddEditPoint(null, vsCMElement.vsCMElementNamespace, Positions.Before, "Authorization_" + TemplateType.Using)
                    .AddEditPoint("ConfigurePageAuthorization", vsCMElement.vsCMElementFunction, Positions.End, "Authorization_" + TemplateType.Main),
                new AhEditProjectItem(AbpProjectType.Web, String.Empty, "{0}WebModule.cs", vsCMElement.vsCMElementOther, "{0}WebModuleConfigureExtensions.ConfigureToolbarOptions(options)", "PageToolbar")
                    .AddEditPoint("ConfigureServices", vsCMElement.vsCMElementFunction, Positions.End, TemplateType.Calling),
                new AhEditProjectItem(AbpProjectType.Web, String.Empty, "{0}WebModule.cs", vsCMElement.vsCMElementOther, "{0}WebModuleConfigureExtensions.ConfigurePageAuthorization(options)", "PageAuthorization")
                    .AddEditPoint("ConfigureServices", vsCMElement.vsCMElementFunction, Positions.End, TemplateType.Calling),
                new AhProjectItem(AbpProjectType.Web, "Pages{3}{2}{3}{1}", "Index.cshtml", "Page.Index"),
                new AhProjectItem(AbpProjectType.Web, "Pages{3}{2}{3}{1}", "Index.cshtml.cs", "Page.IndexCs"),
                new AhProjectItem(AbpProjectType.Web, "Pages{3}{2}{3}{1}", "Index.js", "Page.IndexJs"),
                new AhProjectItem(AbpProjectType.Web, "Pages{3}{2}{3}{1}", "CreateModal.cshtml", "Page.Create"),
                new AhProjectItem(AbpProjectType.Web, "Pages{3}{2}{3}{1}", "CreateModal.cshtml.cs", "Page.CreateCs"),
                new AhProjectItem(AbpProjectType.Web, "Pages{3}{2}{3}{1}", "EditModal.cshtml", "Page.Edit"),
                new AhProjectItem(AbpProjectType.Web, "Pages{3}{2}{3}{1}", "EditModal.cshtml.cs", "Page.EditCs")
            },

            //11. Extra Js Files
            new[] {
                new AhProjectItem(AbpProjectType.Web, "Pages{3}{2}{3}{1}", "CreateModal.js", "Page.CreateJs"),
                new AhProjectItem(AbpProjectType.Web, "Pages{3}{2}{3}{1}", "EditModal.js", "Page.EditJs")
            },
       };

        /// <summary>
        /// Get file's project item information about to be created.
        /// </summary>
        /// <param name="sln"></param>
        /// <param name="abpFile"></param>
        /// <returns></returns>
        public static AhProjectItem[] GetAhProjectItems(this AbpMainFile abpFile, AhEntity entity)
        {
            int idx = (int)abpFile;
            var items = ahProjectItems[idx];
            foreach (var item in items)
            {
                item.SetRealNames(entity.Solution.ModuleName, entity.Name, entity.RelativePath);
            }

            return items;
        }
    }
}
