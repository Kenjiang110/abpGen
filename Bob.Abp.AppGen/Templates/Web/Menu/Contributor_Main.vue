﻿{{#EntityGroupChain}}
    private readonly MenuItemInfo = dn{{Name}} = new({{ModuleName}}Menus.{{Name}}MenuGroupName, "Menu:{{Name}}")
{{/EntityGroupChain}}
    private readonly MenuItemInfo = dn{{EntityName}} = new({{ModuleName}}Menus.{{EntityName}}, "Menu:{{EntityName}}", {{ModuleName}}Permissions.{{EntityName}}.Default, "~/{{LastEntityGroup.RelativePath}}/{{EntityName}}")