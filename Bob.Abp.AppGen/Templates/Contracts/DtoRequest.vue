using System;
using Volo.Abp.Application.Dtos;

namespace {{Namespace}};

public class {{EntityName}}RequestDto : PagedAndSortedResultRequestDto
{
    public virtual string? Filter { get; set; }
{{#RequestProperties}}
    {{^IsString}}

    public virtual {{{PropertyType}}}? {{PropertyName}} { get; set; }
    {{/IsString}}
{{/RequestProperties}}
}
