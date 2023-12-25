
    private void CreateMap{{EntityName}}()
    {
        CreateMap<{{EntityName}}, {{EntityName}}Dto>(){{#IsExtensible}}.MapExtraProperties(){{/IsExtensible}};
    }
