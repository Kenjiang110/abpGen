{{#EntityGroupChain}}
    private readonly MenuItemInfo = {{ToCamel Name}} = new({{ModuleName}}Menus.{{Name}}MenuGroupName, "Menu:{{EntityName}}")
{{/EntityGroupChain}}
    private readonly MenuItemInfo = {{ToCamel EntityName}} = new({{ModuleName}}Menus.{{EntityName}}, "Menu:{{EntityName}}", {{ModuleName}}Permissions.{{EntityName}}.Default, "~/{{LastEntityGroup.RelativePath}}/{{EntityName}}")
