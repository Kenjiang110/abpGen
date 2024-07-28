Using GitHub to maintain an open source project, I am a beginner, so any advice is welcome. 

This project is a tool I developed myself while using the abp vNext framework to build applications, and it serves as a plugin for VS2022. I use Vue templates to generate code, so you can easily change it to generate any code you want.

To get started, please download the source code and build it. 

To build it, you will need to add the VS Extension and WPF development support for VS. 

Then, locate the Bob.Abp.AppGen.vsix file and double-click to install the plugin.

Next, create an Entity class in the Domain project. 

Currently, I can only support AggregateRoot<Guid> and Entity<Guid>, but you can easily modify the generated source code.

After creating the class file (e.g. Country.cs), right-click it and find the "Bob Abp Assistant" menu:
- Extract Information File: This will create a file with the name *.json (e.g. Country.cs.json)
- Edit Information File: Use the UI to modify the JSON file. You can also directly modify this file as it is easy to understand.
- Generate Codes: This will generate multiple code files for the entity, but you can choose which files to generate
- ![right click domain file and choose 'Bob ABP assistant'](https://github.com/user-attachments/assets/3bf7fbde-84cb-455f-bc6d-52daf50452db)
![language resource](https://github.com/user-attachments/assets/b61b804c-6172-4849-831b-8cb206a707c1)
![generate prompt](https://github.com/user-attachments/assets/697d78c8-f6c0-4872-871b-a5a638ec5490)
![edit ui - properties](https://github.com/user-attachments/assets/85b715cb-19df-47df-be72-4a463c729906)
![edit ui - groups](https://github.com/user-attachments/assets/a8a058fd-f1bf-4c2a-a770-95f60234e602)
![edit ui - generations](https://github.com/user-attachments/assets/7e046eb6-7c43-46ae-a5d4-1ecc68ffcab7)
![edit ui - basic](https://github.com/user-attachments/assets/27886c6c-9594-4b8b-bd90-ae64f32f23cc)

 Contracts_Dtos：
	xxxxDto.cs (Application.Contracts)
        xxxxRequestDto.cs (Application.Contracts)
        xxxxCreateOrUpdateDtoBase.cs	(Application.Contracts)
	xxxxCreateDto.cs (Application.Contracts)
	xxxxUpdateDto.cs	(Application.Contracts)

 Contracts_RemoteServiceConsts: project Application.Contracts
	RemoteServiceConsts.cs (Application.Contracts)

 MongoDB_Repository
        xxxxConsts.cs (Domain.Shared, string properties' max length)
        IxxxxRepository.cs (Domain, entity's IRepository interface )
        I****MongoDbContext.cs (MongoDB,  add DbSet<entity> in DbContext interface)
        ****MongoDbContext.cs (MongoDB, add DbSet<entity> in DbContext class)

 EntityFrameworkCore_Repository
        xxxxConsts.cs (Domain.Shared, string properties' max length)
        IxxxxRepository.cs (Domain, entity's IRepository interface )
	xxxxRepository.cs (EntityFrameworkCore, Repository class)
,       I****DbContext.cs (EntityFrameworkCore,  add DbSet<entity> in DbContext interface)
        ****DbContext.cs (EntityFrameworkCore, add DbSet<entity> in DbContext class)
	****DbContext.cs （EntityFrameworkCore， add builder.Configure() to OnModelCreateing())
        ****DbContextModelCreatingExtensions.cs"（EntityFrameworkCore， add ConfigureXxxx() to OnModelCreateing())

 Permission
        ****Permissions.cs (Application.Contracts, Permission consts for entity)
        ****PermissionDefinitionProvider.cs (Application.Contracts, Permission Definition for entity)

 AppService
        IxxxxAppService.cs (Application.Contracts,  entity's IAppService interface)
	****ApplicationAutoMapperProfile.cs (Application, Dto-Entity AutoMapper define)
        xxxxAppService.cs (Application,  entity's AppService  class)

 Web_ViewModel
        ****WebAutoMapperProfile.cs (Web, Dto-ViewModel AutoMapper define)
        xxxxInfoModel.cs (Web, entity's base ViewModel)

 HttpApi_Controller
       xxxxController.cs" (HttpApi, entity's api Controller)

 Web_Menu
       	MenuItemInfo.cs (Web, base menu item info class)
	****Menus.cs (Web, entity's menu consts)
	****MenuContributor.cs (Web, add entity's menu item)

 Web_Pages
       	****WebModuleConfigureExtensions.cs (Web, entity index page's toolbar definition)
       	****WebModuleConfigureExtensions.cs (Web, entity pages' authorization)
       	xxxx/Index.cshtml
	xxxx/Index.cshtml.cs
       	xxxx/Index.js
	xxxx/CreateModal.cshtml
	xxxx/CreateModal.cshtml.cs
	xxxx/EditModal.cshtml
	xxxx/EditModal.cshtml.cs

 Web_Page_ExtraJs, 
        xxxx/CreateModal.js ( Js file to support Extra create Tab)
        xxxx/EditModal.js ( Js file to support Extra edit Tab)
