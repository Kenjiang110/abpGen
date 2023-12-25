{
  "Namespace": "{{Namespace}}",
  "RelativePath": "{{RelativePath}}",
  "Name": "{{EntityName}}",
  "EntityKind": 5,
  "EntityKindsRemark": "0-None, 1-Entity, 2-Hierarcy, 4-Extensible, 8-MultiTent",
  "AllowBatchDelete": false,
  "ExtraUsings": [
    "System.Collections.Generic",
    "Volo.Abp.IdentityServer.ApiResources"
  ],
  "Properties": [
    {
      "PropertyType": "string",
      "PropertyName": "Name",
      "IsEnum": false,
      "IsList": false,
      "MaxLengthOrPercise": null,
      "MinLengthOrScale": null,
      "Required": false,
      "Members": []
    },
  ],
  "ListFields": "",
  "RequestFields": "",
  "CreateFields": "",
  "UpdateFields": "",
  "HiddenFields": "",
  "ReadOnlyFields": "",
  "PropertyGroups": [
    {
      "Title": "",
      "TabType": 0,
      "Properties": "Name"
    }
  ],
  "Languages": [
    {
      "Culture": "en",
      "DisplayName": "{{EntityName}}",
      "NewEntityCmd": "New {{EntityName}}",
      "Properties": {
        "Name": "Name",
      },
      "Enums": {}
    },
    {
      "Culture": "zh-Hans",
      "DisplayName": "{{实体名称}}",
      "NewEntityCmd": "新建{{实体名称}}",
      "Properties": {
        "Name": "名称",
      },
      "Enums": {}
    }
  ],
  "SkipSettings": {
    "Shared_Localization": true,
    "Shared_EntityConsts": true,

    "Contracts_DtoEntity": true,
    "Contracts_DtoRequest": true,
    "Contracts_DtoCreateUpdateBase": false,
    "Contracts_DtoCreate": false,
    "Contracts_DtoUpdate": true,
    "Contracts_IAppService": true,

    "Contracts_RemoteServiceConsts": true,
    "Contracts_PermissionConst": true,
    "Contracts_PermissionDefine": true,

    "Application_AutoMapper": false,
    "Application_AppService": false,
    "HttpApi_Controller": true,

    "Web_Page_ViewModel": true,
    "Web_Page_Index": true,
    "Web_Page_IndexCs": true,
    "Web_Page_IndexJs": true,
    "Web_Page_Create": false,
    "Web_Page_CreateCs": false,
    "Web_Page_CreateJs": false,
    "Web_Page_Edit": false,
    "Web_Page_EditCs": false,
    "Web_Page_EditJs": false,

    "MongoDB_IDbContext": true,
    "MongoDB_DbContext": true,

    "Web_AutoMapper": true,
    "Web_PageToolbar": true,
    "Web_PageAuthorization": true,
    "Web_Menu_Consts": true,
    "Web_Menu_ContributorModify": true,
    "Web_Menu_Contributor": true
  }
}