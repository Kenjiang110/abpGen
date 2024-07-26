using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace {{Namespace}}
{
    public static class {{ModuleName}}DbContextModelCreatingExtensions
    {
        public static void Configur{{ModuleName}}(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));
        }
    }
}
