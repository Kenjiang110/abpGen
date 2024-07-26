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
    "Shared_Localization": false,

    "Contracts_Dtos": false,
    "Contracts_RemoteServiceConsts": true,
    "MongoDB_Repository": true,
    "EntityFrameworkCore_Repository": false,

    "Permission": false,
    "AppService": false,
    "HttpApi_Controller": true,

    "Web_ViewModel": false,
    "Web_Menu": false,
    "Web_Pages": false,
    "Web_Page_ExtraJs": true
  }
}