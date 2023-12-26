{{!#include Using.Template}}
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using {{Namespace}};
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Domain.Entities;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace {{RootNamespace}}.{{ModuleName}}.Web.Pages.{{LastEntityGroup.RelativeNsPath}}.{{EntityName}};

public class EditModalModel : {{ModuleName}}PageModel
{
    [BindProperty]
    public {{EntityName}}InfoModel {{EntityName}} { get; set; } = new {{EntityName}}InfoModel();
{{#UiTabs}}
    {{#IsUpdateTab}}
        {{#IsSimpleList}}

    public {{ToSingle Property.PropertyType}}InfoModel New{{ToSingle Property.PropertyName}} { get; set; }
        {{/IsSimpleList}}
    {{/IsUpdateTab}}
{{/UiTabs}}

    protected I{{EntityName}}AppService {{EntityName}}AppService { get; }

    public EditModalModel(I{{EntityName}}AppService {{Lower.EntityName}}AppService)
    {
        {{EntityName}}AppService = {{Lower.EntityName}}AppService;
    }

    public virtual async Task OnGetAsync(Guid id)
    {
{{^OnlyBasicTabUi}}
        {{EntityName}} = new {{EntityName}}InfoModel { Id = id };
{{/OnlyBasicTabUi}}
{{#OnlyBasicTabUi}}
        {{EntityName}} = ObjectMapper.Map<{{EntityName}}Dto, {{EntityName}}InfoModel>(
            await {{EntityName}}AppService.GetAsync(id)
        );
{{/OnlyBasicTabUi}}
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        ValidateModel();

        var input = ObjectMapper.Map<{{EntityName}}InfoModel, {{EntityName}}UpdateDto>({{EntityName}});
        var dto = await {{EntityName}}AppService.UpdateAsync({{EntityName}}.Id, input);

        return new JsonResult(dto);
    }

    public class {{EntityName}}InfoModel : {{EntityName}}.{{EntityName}}InfoModel, IHasConcurrencyStamp
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [HiddenInput]
        public string ConcurrencyStamp { get; set; } = string.Empty;
{{#UpdateProperties}}
    {{#IsOverrideOrPureUpdateField}}

        {{#IsHiddenField}}
        [HiddenInput]
        {{/IsHiddenField}}
        {{^IsHiddenField}}
            {{#ReadOnlyWhenUpdate}}
        [ReadOnlyInput]
            {{/ReadOnlyWhenUpdate}}
            {{^ReadOnlyWhenUpdate}}
                {{#Required}}
        [Required]
                {{/Required}}
                {{#IsString}}
        [DynamicStringLength(typeof({{EntityName}}Consts), nameof({{EntityName}}Consts.Max{{PropertyName}}Length))]
                {{/IsString}}
            {{/ReadOnlyWhenUpdate}}
        {{/IsHiddenField}}
        public {{#IsCreateField}}override{{/IsCreateField}}{{^IsCreateField}}virtual{{/IsCreateField}} {{#IsEnum}}{{PropertyType}}{{/IsEnum}}{{^IsEnum}}{{{ToModel PropertyType}}}{{/IsEnum}}{{#Nullable}}?{{/Nullable}} {{PropertyName}} { get; set; }{{#IsString}}{{^Nullable}} = string.Empty;{{/Nullable}}{{/IsString}} 
    {{/IsOverrideOrPureUpdateField}}
{{/UpdateProperties}} 
    }
}