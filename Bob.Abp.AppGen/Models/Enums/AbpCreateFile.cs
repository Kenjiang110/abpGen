using Bob.Abp.AppGen.DteExtension;

namespace Bob.Abp.AppGen.Models
{
    /// <summary>
    /// Files to be created.
    /// </summary>
    public enum AbpCreateFile
    {
        Contracts_DtoEntity,
        Contracts_DtoRequest,
        Contracts_DtoCreateUpdateBase,
        Contracts_DtoCreate,
        Contracts_DtoUpdate,
        Contracts_IAppService,
        Contracts_RemoteServiceConsts,

        Domain_IRepository,
        EntityFrameworkCore_Repository,

        Shared_EntityConsts,
        Application_AppService,
        HttpApi_Controller,

        Web_Menu_MenuItemInfo,
        Web_Page_ViewModel,
        Web_Page_Index,
        Web_Page_IndexCs,
        Web_Page_IndexJs,
        Web_Page_Create,
        Web_Page_CreateCs,
        Web_Page_CreateJs,
        Web_Page_Edit,
        Web_Page_EditCs,
        Web_Page_EditJs
    }

    public static class AbpCreateFileExtension
    {
        /// <summary>
        /// {0} is ModuleName
        /// {1} is EntityName for file name and folderPath,
        /// {2} is EntityRelativePath for folderPath ,
        /// {3} is Path.DirectorySeparatorChar
        /// </summary>
        private static readonly AhProjectItem[] ahProjectItems = new AhProjectItem[]
        {
            new AhProjectItem(AbpProjectType.Contracts, "{2}{3}{1}", "{1}Dto.cs"),
            new AhProjectItem(AbpProjectType.Contracts, "{2}{3}{1}", "{1}RequestDto.cs"),
            new AhProjectItem(AbpProjectType.Contracts, "{2}{3}{1}", "{1}CreateOrUpdateDtoBase.cs"),
            new AhProjectItem(AbpProjectType.Contracts, "{2}{3}{1}", "{1}CreateDto.cs"),
            new AhProjectItem(AbpProjectType.Contracts, "{2}{3}{1}", "{1}UpdateDto.cs"),
            new AhProjectItem(AbpProjectType.Contracts, "{2}{3}{1}", "I{1}AppService.cs"),
            new AhProjectItem(AbpProjectType.Contracts, string.Empty, "{0}RemoteServiceConsts.cs", secured: true),

            new AhProjectItem(AbpProjectType.Domain, "{2}", "I{1}Repository.cs"),
            new AhProjectItem(AbpProjectType.EntityFrameworkCore, "{2}", "{1}Repository.cs"),

            new AhProjectItem(AbpProjectType.Shared, "{2}..", "{1}Consts.cs"),
            new AhProjectItem(AbpProjectType.Application, "{2}..", "{1}AppService.cs"),
            new AhProjectItem(AbpProjectType.HttpApi, "{2}..", "{1}Controller.cs"),

            new AhProjectItem(AbpProjectType.Web, "Menus", "MenuItemInfo.cs"),
            new AhProjectItem(AbpProjectType.Web, "Pages{3}{2}{3}{1}", "{1}InfoModel.cs"),
            new AhProjectItem(AbpProjectType.Web, "Pages{3}{2}{3}{1}", "Index.cshtml"),
            new AhProjectItem(AbpProjectType.Web, "Pages{3}{2}{3}{1}", "Index.cshtml.cs"),
            new AhProjectItem(AbpProjectType.Web, "Pages{3}{2}{3}{1}", "Index.js"),
            new AhProjectItem(AbpProjectType.Web, "Pages{3}{2}{3}{1}", "CreateModal.cshtml"),
            new AhProjectItem(AbpProjectType.Web, "Pages{3}{2}{3}{1}", "CreateModal.cshtml.cs"),
            new AhProjectItem(AbpProjectType.Web, "Pages{3}{2}{3}{1}", "CreateModal.js"),
            new AhProjectItem(AbpProjectType.Web, "Pages{3}{2}{3}{1}", "EditModal.cshtml"),
            new AhProjectItem(AbpProjectType.Web, "Pages{3}{2}{3}{1}", "EditModal.cshtml.cs"),
            new AhProjectItem(AbpProjectType.Web, "Pages{3}{2}{3}{1}", "EditModal.js"),
       };

        /// <summary>
        /// Get file's project item information about to be created.
        /// </summary>
        /// <param name="sln"></param>
        /// <param name="abpFile"></param>
        /// <returns></returns>
        public static AhProjectItem GetAhProjectItem(this AbpCreateFile abpFile, AhEntity entity)
        {
            int idx = (int)abpFile;
            var item = ahProjectItems[idx].SetRealNames(entity.Solution.ModuleName, entity.Name, entity.RelativePath);

            return item;
        }
    }
}
