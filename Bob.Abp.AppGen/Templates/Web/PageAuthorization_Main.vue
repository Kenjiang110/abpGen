
        
    private void Configure{{EntityName}}PageAuthorization()
    {
        Configure<RazorPagesOptions>(options =>
        {
            options.Conventions.AuthorizePage("/{{LastEntityGroup.RelativePath}}/{{EntityName}}/Index", {{ModuleName}}Permissions.{{EntityName}}.Default);
            options.Conventions.AuthorizePage("/{{LastEntityGroup.RelativePath}}/{{EntityName}}/CreateModal", {{ModuleName}}Permissions.{{EntityName}}.Create);
            options.Conventions.AuthorizePage("/{{LastEntityGroup.RelativePath}}/{{EntityName}}/EditModal", {{ModuleName}}Permissions.{{EntityName}}.Update);
        });
    }