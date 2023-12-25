abp.modals.{{Lower.LastEntityGroup.Name}}{{EntityName}}Edit = function () {
    var vm = new Vue({
        el: '#{{EntityName}}EditModalWrapper',
        data: {
            {{ToCamel EntityName}}: {},
{{#UiTabs}}
    {{#IsUpdateTab}}
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
    {{/IsUpdateTab}}
{{/UiTabs}}
        },
        methods: {
{{#UiTabs}}
    {{#IsUpdateTab}}
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
    {{/IsUpdateTab}}
{{/UiTabs}}
        }
    });

    function initModal(modalManager, args) {
        var l = abp.localization.getResource('{{ModuleName}}');
        var _{{Lower.EntityName}}AppService = {{Lower.RootNamespace}}.{{Lower.moduleName}}.{{Lower.LastEntityGroup.RelativeNsPath}}.{{Lower.EntityName}};

        var id = $("#{{EntityName}}_Id").val();
        _{{Lower.EntityName}}AppService.get(id).done(function (data) {
            vm.$data.{{ToCamel EntityName}} = data;
{{#UiTabs}}
    {{#IsUpdateTab}}
        {{#IsMultiSelect}}
            {{#Property}}
            //TODO [AbpGen] Set the right api path & mapping
            {{Lower.RootNamespace}}.{{Lower.ModuleName}}.{{ToCamel PropertyType}}.getAllList() //get and set select list
                .done(function (ldata) {
                    vm.$data.{{ToCamel PropertyName}}.all = ldata.items.map(function (item) {
                        let rst = { {{ToCamel Member}}: item.name, selected: false };
                        for(v of data.{{ToCamel PropertyName}}) {
                            if(v.{{ToCamel Member}} == rst.{{ToCamel Member}}) {
                                rst.selected = true;
                                break;
                            } 
                        }
                        return rst;
                    });
                    vm.update{{PropertyName}}();
                });
            {{/Property}}
        {{/IsMultiSelect}}
    {{/IsUpdateTab}}
{{/UiTabs}}
        });

        $('.scrollcard').mCustomScrollbar({
            theme: "minimal-dark"
        });
    };

    return {
        initModal: initModal
    };
};