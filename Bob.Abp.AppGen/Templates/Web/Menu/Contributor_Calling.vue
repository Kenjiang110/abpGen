{{#VariableChain}}
    {{^IsLast}}
        var {{Name}}MenuItem = {{^ParentName}}{{ToCamel ModuleName}}{{/ParentName}}{{#ParentName}}{{.}}{{/ParentName}}MenuItem.AddAndGetSubItem({{ToCamel Name}}, l);
    {{/IsLast}}
    {{#IsLast}}
        {{^ParentName}}{{ToCamel ModuleName}}{{/ParentName}}{{#ParentName}}{{.}}{{/ParentName}}MenuItem.AddAndGetSubItem({{ToCamel Name}}, l);
    {{/IsLast}}
{{#VariableChain}}