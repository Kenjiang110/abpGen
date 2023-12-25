@page
@using {{RootNamespace}}.{{ModuleName}}.Localization
@using {{RootNamespace}}.{{ModuleName}}.Web.Pages.{{LastEntityGroup.RelativeNsPath}}.{{EntityName}}
@using Microsoft.AspNetCore.Mvc.Localization
@using Microsoft.Extensions.Localization
@using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Modal
@using Volo.Abp.Localization
@using Volo.Abp.ObjectExtending
@using Volo.Abp.Data
@model EditModalModel
@inject IHtmlLocalizer<{{ModuleName}}Resource> L
@inject IStringLocalizerFactory StringLocalizerFactory
@{
    Layout = null;
}
<form asp-page="/{{LastEntityGroup.RelativePath}}/{{EntityName}}/EditModal">
    <abp-modal size="Large">
        <abp-modal-header title="@L["Edit"].Value"></abp-modal-header>
        <abp-modal-body id="{{EntityName}}EditModalWrapper">
            <abp-input asp-for="{{EntityName}}.Id" />
            <abp-input asp-for="{{EntityName}}.ConcurrencyStamp" />
        {{^UseTabUi}}
            {{#BasicUiTab}}
                {{#Properties}}
            <abp-{{^IsEnum}}input{{/IsEnum}}{{#IsEnum}}select{{/IsEnum}} asp-for="{{EntityName}}.{{PropertyName}}" />
                {{/Properties}}
            {{/BasicUiTab}}
        {{/UseTabUi}}
        {{#UseTabUi}}
            <abp-tabs>
            {{#UiTabs}}
                {{#IsUpdateTab}}
               <abp-tab title="@L["DisplayName:{{Title}}"].Value">
                    <div class="{{#IsMultiSelect}}row {{/IsMultiSelect}}m-3">
                    {{#IsBasic}}
                        {{#Properties}}
                        <abp-{{^IsEnum}}input{{/IsEnum}}{{#IsEnum}}selct{{/IsEnum}} {{#IsBoolean}}:checked{{/IsBoolean}}{{^IsBoolean}}:value{{/IsBoolean}}="{{Lower.EntityName}}.{{ToCamel PropertyName}}" asp-for="{{EntityName}}.{{PropertyName}}" />
                        {{/Properties}}
                    {{/IsBasic}}
                    {{#IsMultiSelect}}
                        {{#Property}}
                        <div class="col">
                            <abp-card text-color="Dark" border="Primary">
                                <abp-card-header>@L["Assigned"].Value</abp-card-header>
                                <abp-card-body class="scrollcard" style="height: 500px">
                                    <abp-list-group>
                                        <abp-list-group-item href="#" v-for="({{ToSingleCamel PropertyName}}, idx) in {{ToCamel PropertyName}}.assigned" v-on:click="remove{{ToSingle PropertyName}}(idx)">
                                            <span v-text="{{ToSingleCamel PropertyName}}.{{ToCamel Member}}"></span>&nbsp;&rarr;
                                            <input :name="'{{EntityName}}.{{PropertyName}}[' + idx + '].{{Member}}'" :value="{{ToSingleCamel PropertyName}}.{{ToCamel Member}}" type="hidden" />
                                        </abp-list-group-item>
                                    </abp-list-group>
                                </abp-card-body>
                            </abp-card>
                        </div>
                        <div class="col">
                            <abp-card text-color="Dark" border="Success">
                                <abp-card-header>@L["Available"].Value</abp-card-header>
                                <abp-card-body class="scrollcard" style="height: 500px">
                                    <abp-list-group>
                                        <abp-list-group-item href="#" v-for="({{ToSingleCamel PropertyName}}, idx) in {{ToCamel PropertyName}}.unassigned" v-on:click="add{{ToSingle PropertyName}}(idx)">
                                            &larr;&nbsp;<span v-text="{{ToSingleCamel PropertyName}}.{{ToCamel Member}}"></span>
                                        </abp-list-group-item>
                                    </abp-list-group>
                                </abp-card-body>
                            </abp-card>
                        </div>
                        {{/Property}}
                    {{/IsMultiSelect}}
                    {{#IsSimpleList}}
                        {{#Property}}
                        <div class="row">
                            <div class="col">
                            {{#Members}}
                                <abp-input asp-for="New{{ToSingle PropertyName}}.{{.}}" validate-pack="{{ToCamel PropertyName}}" v-model="new{{ToSingle PropertyName}}.{{ToCamel .}}" />
                            {{/Members}}
                            </div>
                        </div>
                        <div class="d-grid gap-2">
                            <abp-button button-type="Primary" icon="plus" v-on:click="add{{ToSingle PropertyName}}">@L["New{{ToSingle PropertyName}}"]</abp-button>
                        </div>
                        <abp-list-group>
                            <abp-list-group-item v-for="({{ToSingleCamel PropertyName}}, idx) in {{ToCamel EntityName}}.{{ToCamel PropertyName}}">
                                <div class="input-group">
                                {{#Members}}
                                    <input :name="'{{EntityName}}.{{PropertyName}}[' + idx + '].{{.}}'" :value="{{ToSingleCamel PropertyName}}.{{ToCamel .}}" class="form-control" readonly />
                                {{/Members}}
                                    <abp-button icon="trash-alt" button-type="Danger" v-on:click="remove{{ToSingle PropertyName}}(idx)" />
                                </div>
                            </abp-list-group-item>
                        </abp-list-group>
                        {{/Property}}
                    {{/IsSimpleList}}
                    </div>
                </abp-tab>
                {{/IsUpdateTab}}
           {{/UiTabs}}
            </abp-tabs>
        {{/UseTabUi}}
        </abp-modal-body>
        <abp-modal-footer buttons="@(AbpModalButtons.Cancel|AbpModalButtons.Save)"></abp-modal-footer>
    </abp-modal>
</form>
