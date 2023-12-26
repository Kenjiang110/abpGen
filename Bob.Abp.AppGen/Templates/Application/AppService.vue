{{!#include Using.Template}}
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
{{#IsExtensible}}
using Volo.Abp.ObjectExtending;
{{/IsExtensible}}
using {{RootNamespace}}.{{ModuleName}}.Permissions;

namespace {{Namespace}};

[Authorize({{ModuleName}}Permissions.{{EntityName}}.Default)]
public class {{EntityName}}AppService : {{ModuleName}}AppService, I{{EntityName}}AppService
{
    protected I{{EntityName}}Repository {{EntityName}}Repository => LazyServiceProvider.LazyGetRequiredService<I{{EntityName}}Repository>();

    public virtual async Task<{{EntityName}}Dto> GetAsync(Guid id)
    {
        return ObjectMapper.Map<{{EntityName}}, {{EntityName}}Dto>(
            await {{EntityName}}Repository.GetAsync(id)
        );
    }

    public virtual async Task<PagedResultDto<{{EntityName}}Dto>> GetListAsync({{EntityName}}RequestDto input)
    {
        var list = await {{EntityName}}Repository.GetListAsync(
            input.Sorting,
            input.SkipCount,
            input.MaxResultCount,
    {{#HasStringRequestField}}
            input.Filter{{#HasNotStringRequestField}},{{/HasNotStringRequestField}}
    {{/HasStringRequestField}}
    {{#RequestNotStringProperties}}
            input.{{PropertyName}}{{^IsLast}},{{/IsLast}}
    {{/RequestNotStringProperties}}
        );

        var totalCount = await {{EntityName}}Repository.GetCountAsync(
    {{#HasStringRequestField}}
            input.Filter{{#HasNotStringRequestField}},{{/HasNotStringRequestField}}
    {{/HasStringRequestField}}
    {{#RequestNotStringProperties}}
            input.{{PropertyName}}{{^IsLast}},{{/IsLast}}
    {{/RequestNotStringProperties}}        );

        return new PagedResultDto<{{EntityName}}Dto>(
            totalCount,
            ObjectMapper.Map<List<{{EntityName}}>, List<{{EntityName}}Dto>>(list)
            );
    }

    [Authorize({{ModuleName}}Permissions.{{EntityName}}.Create)]
    public virtual async Task<{{EntityName}}Dto> CreateAsync({{EntityName}}CreateDto input)
    {
        var {{Lower.EntityName}} = new {{EntityName}}(GuidGenerator.Create(),
    {{#CreateProperties}}
            {{ToCamel PropertyName}}: input.{{PropertyName}}{{^IsLast}},{{/IsLast}}{{#IsLast}});{{/IsLast}}
    {{/CreateProperties}}
    {{#IsExtensible}}
        input.MapExtraPropertiesTo({{Lower.EntityName}});
    {{/IsExtensible}} 
        {{Lower.EntityName}} = await {{EntityName}}Repository.InsertAsync({{Lower.EntityName}}, true);

        return ObjectMapper.Map<{{EntityName}}, {{EntityName}}Dto>({{Lower.EntityName}});
    }

    [Authorize({{ModuleName}}Permissions.{{EntityName}}.Update)]
    public virtual async Task<{{EntityName}}Dto> UpdateAsync(Guid id, {{EntityName}}UpdateDto input)
    {
        var {{Lower.EntityName}} = await {{EntityName}}Repository.GetAsync(id, false);
{{#IsExtensible}}
        {{Lower.EntityName}}.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);
        input.MapExtraPropertiesTo({{Lower.EntityName}});
{{/IsExtensible}}
{{#UpdateProperties}}
    {{^ReadOnlyWhenUpdate}}
        {{^IsList}}
            {{#PublicSetter}}
        {{Lower.EntityName}}.{{PropertyName}} = input.{{PropertyName}};
            {{/PublicSetter}}
            {{^PublicSetter}}
        {{Lower.EntityName}}.Set{{PropertyName}}(input.{{PropertyName}});
            {{/PublicSetter}}
        {{/IsList}}
        {{#IsList}}
        {{Lower.EntityName}}.Set{{PropertyName}}(input.{{PropertyName}});                
        {{/IsList}}
    {{/ReadOnlyWhenUpdate}}
{{/UpdateProperties}}

        {{Lower.EntityName}} = await {{EntityName}}Repository.UpdateAsync({{Lower.EntityName}}, true);
        return ObjectMapper.Map<{{EntityName}}, {{EntityName}}Dto>({{Lower.EntityName}});
    }

    [Authorize({{ModuleName}}Permissions.{{EntityName}}.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await {{EntityName}}Repository.DeleteAsync(id);
    }
}
