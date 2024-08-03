{{#VariableChain}}
    {{^IsLast}}
        var {{Name}}MenuItem = {{^ParentName}}{{ToCamel ModuleName}}{{/ParentName}}{{#ParentName}}{{.}}{{/ParentName}}MenuItem.AddAndGetSubItem({{Name}}, l);
    {{/IsLast}}
    {{#IsLast}}
        {{^ParentName}}{{ToCamel ModuleName}}{{/ParentName}}{{#ParentName}}{{.}}{{/ParentName}}MenuItem.AddAndGetSubItem({{Name}}, l);
    {{/IsLast}}
{{#VariableChain}}