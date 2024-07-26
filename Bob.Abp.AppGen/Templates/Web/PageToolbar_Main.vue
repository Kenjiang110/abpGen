    
    
    private void Configure{{EntityName}}PageToolbar()
    {
        Configure<AbpPageToolbarOptions>(options =>
        {
            options.Configure<Pages.{{LastEntityGroup.RelativeNsPath}}.{{EntityName}}.IndexModel>(
                toolbar =>
                {
                    toolbar.AddButton(
                        LocalizableString.Create<{{ModuleName}}Resource>("New{{EntityName}}"),
                        icon: "plus", name: "Create{{EntityName}}",
                        requiredPolicyName: {{ModuleName}}Permissions.{{EntityName}}.Create
                   {{#HasRequestField}}
                    ).AddButton(
                        LocalizableString.Create<{{ModuleName}}Resource>("Search"),
                        icon: "search", name: "Query{{EntityName}}",
                        requiredPolicyName: {{ModuleName}}Permissions.{{EntityName}}.Default
                   {{/HasRequestField}}
                   {{#AllowBatchDelete}}
                    ).AddButton(
                        LocalizableString.Create<{{ModuleName}}Resource>("Delete"),
                        icon: "trash", name: "Delete{{EntityName}}",
                        requiredPolicyName: {{ModuleName}}Permissions.{{EntityName}}.Delete
                   {{/AllowBatchDelete}}
                    );
                }
            );
        });
    }