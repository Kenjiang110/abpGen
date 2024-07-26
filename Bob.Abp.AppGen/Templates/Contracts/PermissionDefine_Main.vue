    private void Define{{EntityName}}(PermissionGroupDefinition myGroup)
    {
        var {{Lower.EntityName}} = myGroup.AddPermission({{ModuleName}}Permissions.{{EntityName}}.Default, L("Menu:{{EntityName}}"));
        {{Lower.EntityName}}.AddChild({{ModuleName}}Permissions.{{EntityName}}.Create, L("Permission:Create"));
        {{Lower.EntityName}}.AddChild({{ModuleName}}Permissions.{{EntityName}}.Update, L("Permission:Edit"));
        {{Lower.EntityName}}.AddChild({{ModuleName}}Permissions.{{EntityName}}.Delete, L("Permission:Delete"));
{{#IsHierarcy}}
        {{Lower.EntityName}}.AddChild({{ModuleName}}Permissions.{{EntityName}}.Move, L("Permission:Move"));
{{/IsHierarcy}}
    }

