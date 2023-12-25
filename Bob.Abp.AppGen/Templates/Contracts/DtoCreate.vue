{{!#include Using.Template}}

namespace {{Namespace}};

public class {{EntityName}}CreateDto : {{EntityName}}CreateOrUpdateDtoBase
{
{{#PureCreateProperties}}
    public virtual {{{ToDto PropertyType}}} {{PropertyName}} { get; set; }{{#IsString}}{{^Nullable}} = string.Empty;{{/Nullable}}{{/IsString}}
    {{^IsLast}}

    {{/IsLast}}
{{/PureCreateProperties}} 
}

