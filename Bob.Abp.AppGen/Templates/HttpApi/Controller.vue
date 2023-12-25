using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using {{RootNamespace}}.{{ModuleName}}.Controllers;

namespace {{Namespace}};

[RemoteService(Name = {{ModuleName}}RemoteServiceConsts.RemoteServiceName)]
[Area({{ModuleName}}RemoteServiceConsts.ModuleName)]
[ControllerName("{{EntityName}}")]
[Route("api/{{Lower.ModuleName}}/{{Lower.LastEntityGroup.RelativePath}}/{{DashedName}}")]
public class {{EntityName}}Controller : {{ModuleName}}Controller, I{{EntityName}}AppService
{
    protected I{{EntityName}}AppService {{EntityName}}AppService { get; }

    public {{EntityName}}Controller(I{{EntityName}}AppService {{Lower.EntityName}}AppService)
    {
        {{EntityName}}AppService = {{Lower.EntityName}}AppService;
    }

    [HttpPost]
    public Task<{{EntityName}}Dto> CreateAsync({{EntityName}}CreateDto input)
    {
       return {{EntityName}}AppService.CreateAsync(input);
    }

    [HttpDelete]
    [Route("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return {{EntityName}}AppService.DeleteAsync(id);
    }

    [HttpGet]
    [Route("{id}")]
    public Task<{{EntityName}}Dto> GetAsync(Guid id)
    {
        return {{EntityName}}AppService.GetAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<{{EntityName}}Dto>> GetListAsync({{EntityName}}RequestDto input)
    {
        return {{EntityName}}AppService.GetListAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public Task<{{EntityName}}Dto> UpdateAsync(Guid id, {{EntityName}}UpdateDto input)
    {
        return {{EntityName}}AppService.UpdateAsync(id, input);
    }
}