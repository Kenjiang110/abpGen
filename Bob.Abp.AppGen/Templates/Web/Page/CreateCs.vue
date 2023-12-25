{{!#include Using.Template}}
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using {{Namespace}};
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;

namespace {{RootNamespace}}.{{ModuleName}}.Web.Pages.{{LastEntityGroup.RelativeNsPath}}.{{EntityName}};

public class CreateModalModel : {{ModuleName}}PageModel
{
    [BindProperty]
    public {{EntityName}}InfoModel {{EntityName}} { get; set; } = new {{EntityName}}InfoModel();
{{#UiTabs}}
    {{#IsCreateTab}}
        {{#IsSimpleList}}

    public {{ToSingle Property.PropertyType}}InfoModel New{{ToSingle Property.PropertyName}} { get; set; }
        {{/IsSimpleList}}
    {{/IsCreateTab}}
{{/UiTabs}}

    protected I{{EntityName}}AppService {{EntityName}}AppService { get; }

    public CreateModalModel(I{{EntityName}}AppService {{Lower.EntityName}}AppService)
    {
        {{EntityName}}AppService = {{Lower.EntityName}}AppService;
    }

    public virtual Task<IActionResult> OnGetAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        ValidateModel();

        var input = ObjectMapper.Map<{{EntityName}}InfoModel, {{EntityName}}CreateDto>({{EntityName}});
        var dto = await {{EntityName}}AppService.CreateAsync(input);

        return new JsonResult(dto);
    }

    public class {{EntityName}}InfoModel : {{EntityName}}.{{EntityName}}InfoModel
    {
{{#PureCreateProperties}}
    {{#Required}}
        [Required]
    {{/Required}}
    {{#IsString}}
        [DynamicStringLength(typeof({{EntityName}}Consts), nameof({{EntityName}}Consts.Max{{PropertyName}}Length))]
    {{/IsString}}
        public virtual {{#IsEnum}}{{PropertyType}}{{/IsEnum}}{{^IsEnum}}{{{ToModel PropertyType}}}{{/IsEnum}}{{#Nullable}}?{{/Nullable}} {{PropertyName}} { get; set; }{{#IsString}}{{^Nullable}} = string.Empty;{{/Nullable}}{{/IsString}}
    {{^IsLast}}
        
    {{/IsLast}}
{{/PureCreateProperties}} 
    }
}