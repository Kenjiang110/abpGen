{{!#include Using.Template}}
using {{Namespace}};
using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;
using System.ComponentModel.DataAnnotations;

namespace {{RootNamespace}}.{{ModuleName}}.Web.Pages.{{LastEntityGroup.RelativeNsPath}}.{{EntityName}};

public class {{EntityName}}InfoModel{{#IsExtensible}} : ExtensibleObject{{/IsExtensible}}
{
{{#BasicProperties}}
    {{#Required}}
    [Required]
    {{/Required}}
    {{#IsString}}
    [DynamicStringLength(typeof({{EntityName}}Consts), nameof({{EntityName}}Consts.Max{{PropertyName}}Length))]
    {{/IsString}}
    public virtual {{#IsEnum}}{{PropertyType}}{{/IsEnum}}{{^IsEnum}}{{{ToModel PropertyType}}}{{/IsEnum}}{{#Nullable}}?{{/Nullable}} {{PropertyName}} { get; set; }{{#IsString}}{{^Nullable}} = string.Empty;{{/Nullable}}{{/IsString}}
    {{^IsLast}}

    {{/IsLast}}
{{/BasicProperties}}
}
