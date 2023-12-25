    private void CreateMap{{EntityName}}()
    {
{{#IsExtensible}}
        //EditModal.Get
        CreateMap<{{EntityName}}Dto, Pages.{{LastEntityGroup.RelativeNsPath}}.{{EntityName}}.EditModalModel.{{EntityName}}InfoModel>()
            .MapExtraProperties();
        //CreateModal
        CreateMap<Pages.{{LastEntityGroup.RelativeNsPath}}.{{EntityName}}.CreateModalModel.{{EntityName}}InfoModel, {{EntityName}}CreateDto>()
            .MapExtraProperties();
        //EditModal
        CreateMap<Pages.{{LastEntityGroup.RelativeNsPath}}.{{EntityName}}.EditModalModel.{{EntityName}}InfoModel, {{EntityName}}UpdateDto>()
             .MapExtraProperties();
{{/IsExtensible}}
{{^IsExtensible}}
        CreateMap<{{EntityName}}Dto, Pages.{{LastEntityGroup.RelativeNsPath}}.{{EntityName}}.{{EntityName}}InfoModel>();
        CreateMap<Pages.{{LastEntityGroup.RelativeNsPath}}.{{EntityName}}.{{EntityName}}InfoModel, {{EntityName}}Dto>();
{{/IsExtensible}}
    }
