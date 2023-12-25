{{#VariableChain}}
    {{^IsLast}}
        var {{Name}}MenuItem = {{Name}}.CreateApplicationMenuItem(l);
        {{^ParentName}}{{ToCamel ModuleName}}{{/ParentName}}{{#ParentName}}{{.}}{{/ParentName}}MenuItem.AddItem({{Name}}MenuItem);
    {{/IsLast}}
    {{#IsLast}}
        {{^ParentName}}{{ToCamel ModuleName}}{{/ParentName}}{{#ParentName}}{{.}}{{/ParentName}}MenuItem.AddItem({{Name}}.CreateApplicationMenuItem(l));
    {{/IsLast}}
{{#VariableChain}}