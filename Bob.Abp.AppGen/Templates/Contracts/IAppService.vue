using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace {{Namespace}};

public interface I{{EntityName}}AppService
    : ICrudAppService<
        {{EntityName}}Dto,
        Guid,
        {{EntityName}}RequestDto,
        {{EntityName}}CreateDto,
        {{EntityName}}UpdateDto>
{
}
