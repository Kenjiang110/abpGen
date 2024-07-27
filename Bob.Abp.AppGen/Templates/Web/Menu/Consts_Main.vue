{{#EntityGroupChain}}
    public const string = {{Name}}MenuGroupName = {{^ParentName}}Prefix{{/ParentName}}{{#ParentName}}{{.}}MenuGroupName{{/ParentName}} + ".{{Name}}"
{{/EntityGroupChain}}
    public const string = {{EntityName}} = {{LastEntityGroup.Name}}MenuGroupName + ".{{EntityName}}"