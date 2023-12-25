{{!#include Using.Template}}
using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace {{Namespace}};

public class {{EntityName}}Dto : {{#IsExtensible}}ExtensibleEntityDto<Guid>, IHasConcurrencyStamp{{/IsExtensible}}{{^IsExtensible}}EntityDto<Guid>{{/IsExtensible}}
{
{{#AllProperties}}
    public virtual {{#IsEnum}}{{PropertyType}}{{/IsEnum}}{{^IsEnum}}{{{ToDto PropertyType}}}{{/IsEnum}}{{#Nullable}}?{{/Nullable}} {{PropertyName}} { get; set; }{{#IsString}}{{^Nullable}} = string.Empty;{{/Nullable}}{{/IsString}}
    {{^IsLast}}

    {{/IsLast}}
{{/AllProperties}}
{{#IsExtensible}}

    public string ConcurrencyStamp { get; set; } = string.Empty;
{{/IsExtensible}}
}