using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace {{Namespace}}
{
    public interface I{{EntityName}}Repository : IRepository<{{EntityName}}, Guid>
    {
        Task<List<{{EntityName}}>> GetListAsync(string? sorting, int skipCount, int maxResultCount,
            string? filter = null,
    {{#RequestNotStringProperties}}
            {{{PropertyType}}}? {{ToCamel PropertyName}} = null,
    {{/RequestNotStringProperties}}
            bool includeDetails = false, CancellationToken cancellationToken = default);

        Task<long> GetCountAsync(string? filter = null,
    {{#RequestNotStringProperties}}
            {{{PropertyType}}}? {{ToCamel PropertyName}} = null,
    {{/RequestNotStringProperties}}
            CancellationToken cancellationToken = default);
    }
}
