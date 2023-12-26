using {{RootNamespace}}.{{ModuleName}}.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace {{Namespace}};

public class {{EntityName}}Repository(IDbContextProvider<{{ModuleName}}DbContext> dbContextProvider) 
    : EfCoreRepository<{{ModuleName}}DbContext, {{EntityName}}, Guid>(dbContextProvider), I{{EntityName}}Repository
{
    public async virtual Task<IQueryable<{{EntityName}}>> GetQueryableAsync({{#HasStringRequestField}}string? filter,{{/HasStringRequestField}}
    {{#RequestNotStringProperties}}
        {{{PropertyType}}}? {{ToCamel PropertyName}},
    {{/RequestNotStringProperties}}
        bool includeDetails = false)
    {
        var queryable = includeDetails ? (await WithDetailsAsync()) : (await base.GetQueryableAsync());

        return queryable
    {{#RequestNotStringProperties}}
            .WhereIf({{ToCamel PropertyName}}.HasValue, x => x.{{PropertyName}} == {{ToCamel PropertyName}}!.Value)
    {{/RequestNotStringProperties}}
    {{#HasStringRequestField}}
            .WhereIf(!filter.IsNullOrWhiteSpace(), x => 
        {{#RequestStringProperties}}
                {{^IsFirst}}|| {{/IsFirst}}x.{{PropertyName}}.Contains(filter!){{#IsLast}});{{/IsLast}}
        {{/RequestStringProperties}}
    {{/HasStringRequestField}}
    }

    public async virtual Task<List<{{EntityName}}>> GetListAsync(string? sorting, int skipCount, int maxResultCount,
    {{#HasStringRequestField}}
            string? filter = null,
    {{/HasStringRequestField}}
    {{#RequestNotStringProperties}}
            {{{PropertyType}}}? {{ToCamel PropertyName}} = null,
    {{/RequestNotStringProperties}}
    bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync(filter{{#RequestNotStringProperties}}, {{ToCamel PropertyName}}{{/RequestNotStringProperties}}, includeDetails))
             .OrderBy(sorting == null || sorting.IsNullOrWhiteSpace() ? "Id" : sorting)
             .PageBy(skipCount, maxResultCount)
             .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<long> GetCountAsync({{#HasStringRequestField}}string? filter = null,{{/HasStringRequestField}}
    {{#RequestNotStringProperties}}
            {{{PropertyType}}}? {{ToCamel PropertyName}} = null,
    {{/RequestNotStringProperties}}
        CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync(filter{{#RequestNotStringProperties}}, {{ToCamel PropertyName}}{{/RequestNotStringProperties}}))
            .LongCountAsync(GetCancellationToken(cancellationToken));
    }
}
