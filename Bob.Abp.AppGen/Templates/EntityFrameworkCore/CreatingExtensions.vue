using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace {{RootNamespace}}.{{ModuleName}}.EntityFrameworkCore
{
    public static class {{ModuleName}}DbContextModelCreatingExtensions
    {
        public static void Configure{{ModuleName}}(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));
        }
    }
}
