{{!#include Using.Template}}
using Volo.Abp.ObjectExtending;

namespace {{Namespace}};

public class {{EntityName}}CreateOrUpdateDtoBase{{#IsExtensible}} : ExtensibleObject{{/IsExtensible}}
{        
{{#BasicProperties}}
    public virtual {{{ToDto PropertyType}}}{{#Nullable}}?{{/Nullable}} {{PropertyName}} { get; set; }{{#IsString}}{{^Nullable}} = string.Empty;{{/Nullable}}{{/IsString}}
    {{^IsLast}}

    {{/IsLast}}
{{/BasicProperties}} 
}