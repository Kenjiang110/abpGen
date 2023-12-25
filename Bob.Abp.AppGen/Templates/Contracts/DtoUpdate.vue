{{!#include Using.Template}}
using Volo.Abp.Domain.Entities;

namespace {{Namespace}};

public class {{EntityName}}UpdateDto : {{EntityName}}CreateOrUpdateDtoBase, IHasConcurrencyStamp
{
{{#PureUpdateProperties}}
    public virtual {{{ToDto PropertyType}}} {{PropertyName}} { get; set; }{{#IsString}}{{^Nullable}} = string.Empty;{{/Nullable}}{{/IsString}}
    {{^IsLast}}

    {{/IsLast}}
{{/PureUpdateProperties}}

    public string ConcurrencyStamp { get; set; } = string.Empty;
}

