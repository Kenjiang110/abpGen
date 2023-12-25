abp.modals.{{Lower.LastEntityGroup.Name}}{{EntityName}}Create = function () {
    var vm = new Vue({
        el: '#{{EntityName}}CreateModalWrapper',
        data: {
            {{ToCamel EntityName}}: { {{#UiTabs}}{{#IsCreateTab}}{{#IsSimpleList}}{{#Property}}{{ToCamel PropertyName}}:[],{{/Property}}{{/IsSimpleList}}{{/IsCreateTab}}{{/UiTabs}} },
{{#UiTabs}}
    {{#IsCreateTab}}
        {{#IsMultiSelect}}
            {{ToCamel Property.PropertyName}}: {
                assigned: [],
                unassigned: [],
                all: [],
            },
        {{/IsMultiSelect}}
        {{#IsSimpleList}}
            new{{ToSingle Property.PropertyName}}: {},
        {{/IsSimpleList}}
    {{/IsCreateTab}}
{{/UiTabs}}
        },
        methods: {
{{#UiTabs}}
    {{#IsCreateTab}}
        {{#IsMultiSelect}}
            {{#Property}}

            add{{ToSingle PropertyName}}: function (idx) {
                this.{{ToCamel PropertyName}}.unassigned[idx].selected = true;
                this.update{{PropertyName}}();
            },
            remove{{ToSingle PropertyName}}: function (idx) {
                this.{{ToCamel PropertyName}}.assigned[idx].selected = false;
                this.update{{PropertyName}}();
            },
            update{{PropertyName}}: function () {
                this.{{ToCamel PropertyName}}.assigned = this.{{ToCamel PropertyName}}.all.filter(function (item) { if (item.selected) return item; });
                this.{{ToCamel PropertyName}}.unassigned = this.{{ToCamel PropertyName}}.all.filter(function (item) { if (!item.selected) return item; });
            },
            {{/Property}}
        {{/IsMultiSelect}}
        {{#IsSimpleList}}
            {{#Property}}

            add{{ToSingle PropertyName}}: function () {
                if ($("form").valid("{{ToCamel PropertyName}}")) {
                    this.{{ToCamel EntityName}}.{{ToCamel PropertyName}}.push({ {{#Members}} {{ToCamel .}}: this.new{{ToSingle PropertyName}}.{{ToCamel .}},{{/Members}} });
                }
            },
            remove{{ToSingle PropertyName}}: function (idx) {
                this.{{ToCamel EntityName}}.{{ToCamel PropertyName}}.splice(idx, 1);
            },
            {{/Property}}
        {{/IsSimpleList}}
    {{/IsCreateTab}}
{{/UiTabs}}
        }
    });

    function initModal(modalManager, args) {
        var l = abp.localization.getResource('{{ModuleName}}');
{{#UiTabs}}
    {{#IsCreateTab}}
        {{#IsMultiSelect}}
            {{#Property}}
        //TODO [AbpGen] Set the right api path & mapping
        {{Lower.RootNamespace}}.{{Lower.ModuleName}}.{{ToCamel PropertyType}}.getAllList() //get and set select list
            .done(function (data) {
                vm.$data.{{ToCamel PropertyName}}.all = data.items.map(function (item) {
                    return { {{ToCamel Member}}: item.name, selected: false };
                });
                vm.update{{PropertyName}}();
            });
            {{/Property}}
        {{/IsMultiSelect}}
    {{/IsCreateTab}}
{{/UiTabs}}

        $('.scrollcard').mCustomScrollbar({
            theme: "minimal-dark"
        });
    };

    return {
        initModal: initModal
    };
};