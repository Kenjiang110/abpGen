@page
@using Bob.Abp.AspNetCore.Mvc.UI.Theme.Shared.Bundling
@using Microsoft.AspNetCore.Authorization
@using Microsoft.AspNetCore.Mvc.Localization
@using Volo.Abp.AspNetCore.Mvc.UI.Layout
@using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Pages.Shared.Components.AbpPageToolbar
@using {{RootNamespace}}.{{ModuleName}}
@using {{RootNamespace}}.{{ModuleName}}.Localization
@using {{RootNamespace}}.{{ModuleName}}.Web.Menus
@using {{RootNamespace}}.{{ModuleName}}.Web.Pages.{{LastEntityGroup.RelativeNsPath}}.{{EntityName}}
@model IndexModel
@inject IHtmlLocalizer<{{ModuleName}}Resource> L
@inject IAuthorizationService Authorization
@inject IPageLayout PageLayout
@{
    PageLayout.Content.Title = L["Menu:{{EntityName}}"].Value;
    PageLayout.Content.BreadCrumb.Add(L["Menu:{{EntityName}}"].Value);
    PageLayout.Content.MenuItemName = {{ModuleName}}Menus.{{EntityName}};
}
@section styles {
<abp-style-bundle name="@typeof(IndexModel).FullName">
{{#IsHierarcy}}
    <abp-style type="typeof(JsTreeStyleContributor)" />
{{/IsHierarcy}}
{{^IsHierarcy}}
    {{#AllowBatchDelete}}
    <abp-style src="/libs/datatables.net/css/select.dataTables.css" />
    {{/AllowBatchDelete}}
{{/IsHierarcy}}
</abp-style-bundle>
}
@section scripts {
<abp-script-bundle name="@typeof(IndexModel).FullName">
{{#IsHierarcy}}
    <abp-script type="typeof(JsTreeExtensionScriptContributor)" />
{{/IsHierarcy}}
{{^IsHierarcy}}
    {{#AllowBatchDelete}}
    <abp-script src="/libs/datatables.net/js/dataTables.select.js" />
    {{/AllowBatchDelete}}
{{/IsHierarcy}}
    <abp-script src="/Pages/{{LastEntityGroup.RelativePath}}/{{EntityName}}/index.js" />
</abp-script-bundle>
}
<abp-card id="{{EntityName}}Wrapper">
    <abp-card-header>
        <abp-row>
            <abp-column size-md="_6">
                <abp-card-title>@L["Menu:{{EntityName}}"]</abp-card-title>
            </abp-column>
            <abp-column size-md="_6" class="text-end">
                @await Component.InvokeAsync(typeof(AbpPageToolbarViewComponent), new { pageName = typeof(IndexModel).FullName })
            </abp-column>
        </abp-row>
    </abp-card-header>
    <abp-card-body>
{{^IsHierarcy}}
        <abp-table striped-rows="true" class="nowrap"></abp-table>
{{/IsHierarcy}}
{{#IsHierarcy}}
        <div id="{{Lower.EntityName}}Tree"></div>
        <div id="{{Lower.EntityName}}TreeEmptyInfo" class="text-muted">@L["NoTreeItem"].Value</div>
{{/IsHierarcy}}
    </abp-card-body>
</abp-card>