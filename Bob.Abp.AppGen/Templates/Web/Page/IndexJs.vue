(function ($) {
    var l = abp.localization.getResource('{{ModuleName}}');

    var _{{Lower.EntityName}}AppService = {{Lower.RootNamespace}}.{{Lower.moduleName}}.{{Lower.LastEntityGroup.RelativeNsPath}}.{{Lower.EntityName}};
    var _editModal = new abp.ModalManager({
        viewUrl: abp.appPath + '{{ModuleName}}/{{LastEntityGroup.RelativePath}}/{{EntityName}}/EditModal',
    {{^OnlyBasicTabUi}}
        modalClass: '{{Lower.LastEntityGroup.Name}}{{EntityName}}Edit',
        scriptUrl: '/Pages/{{ModuleName}}/{{LastEntityGroup.RelativePath}}/{{EntityName}}/EditModal.js', //Lazy Load URL''
    {{/OnlyBasicTabUi}}
    });
    var _createModal = new abp.ModalManager({
        viewUrl: abp.appPath + '{{ModuleName}}/{{LastEntityGroup.RelativePath}}/{{EntityName}}/CreateModal',
    {{^OnlyBasicTabUi}}
        modalClass: '{{Lower.LastEntityGroup.Name}}{{EntityName}}Create',
        scriptUrl: '/Pages/{{ModuleName}}/{{LastEntityGroup.RelativePath}}/{{EntityName}}/CreateModal.js', //Lazy Load URL''
    {{/OnlyBasicTabUi}}
    });
{{^IsHierarcy}}
    {{#HasRequestField}}

    var inputAction = function (requestData, dataTableSettings) {
        return {
    {{#HasStringRequestField}}
            filter: $('#Input_Filter').val(),
    {{/HasStringRequestField}}
    {{#RequestNotStringProperties}}
            {{ToCamel PropertyName}}: $('#Input_{{PropertyName}}').val(),
    {{/RequestNotStringProperties}}
        };
    };
   {{/HasRequestField}}

    var _dataTable = null;

    abp.ui.extensions.entityActions.get('{{Lower.LastEntityGroup.RelativeNsPath}}.{{Lower.EntityName}}').addContributor(
        function (actionList) {
            return actionList.addManyTail(
                [
                    {
                        text: l('Edit'),
                        visible: abp.auth.isGranted('{{ModuleName}}.{{EntityName}}.Update'),
                        action: function (data) {
                            _editModal.open({ id: data.record.id, });
                        },
                    },
                    {
                        text: l('Delete'),
                        visible: function (data) {
                            return (
                                !data.isStatic &&
                                abp.auth.isGranted('{{ModuleName}}.{{EntityName}}.Delete')
                            );
                        },
                        confirmMessage: function (data) {
                            return l('ItemWillBeDeletedMessageWithFormat', data.record.name);
                        },
                        action: function (data) {
                            _{{Lower.EntityName}}AppService.delete(data.record.id)
                                .then(function () {
                                    _dataTable.ajax.reload();
                                });
                        },
                    }
                ]
            );
        }
    );

    abp.ui.extensions.tableColumns.get('{{Lower.LastEntityGroup.RelativeNsPath}}.{{Lower.EntityName}}').addContributor(
        function (columnList) {
            columnList.addManyTail(
                [
    {{#AllowBatchDelete}}
                  {
                        title: '<input id="chkSelectAll" type="checkbox" />',
                        orderable: false,
                        className: 'select-checkbox',
                        defaultContent: ''
                    },
    {{/AllowBatchDelete}}
    {{#ListProperties}}
                    {
                        title: l('DisplayName:{{EntityName}}:{{PropertyName}}'),
                        data: '{{ToCamel PropertyName}}',
        {{#IsEnum}}
                        render : function(data, type, row) {
                            var names = [{{#Members}}l("Enum:{{EntityName}}.{{.}}"), {{/Members}}];
                            return names[data];
                        }
        {{/IsEnum}}
        {{^IsEnum}}
            {{#DataFormat}}
                        dataFormat: '{{.}}'
            {{/DataFormat}}
        {{/IsEnum}}
                    },
    {{/ListProperties}}
                    {
                        title: l("Actions"),
                        rowAction: {
                            items: abp.ui.extensions.entityActions.get('{{Lower.LastEntityGroup.RelativeNsPath}}.{{Lower.EntityName}}').actions.toArray()
                        }
                    },
                ]
            );
        },
        0 //adds as the first contributor
    );

    $(function () {
        var _$wrapper = $('#{{EntityName}}Wrapper');
        var _$table = _$wrapper.find('table');

        _dataTable = _$table.DataTable(
            abp.libs.datatables.normalizeConfiguration({
                order: [[1, 'asc']],
                searching: {{#HasRequestField}}false{{/HasRequestField}}{{^HasRequestField}}true{{/HasRequestField}},
                processing: true,
                serverSide: true,
                scrollX: true,
                paging: true,
                ajax: abp.libs.datatables.createAjax(
                    _{{Lower.EntityName}}AppService.getList,
               {{#HasRequestField}}
                    inputAction
               {{/HasRequestField}}
                ),
                columnDefs: abp.ui.extensions.tableColumns.get('{{Lower.LastEntityGroup.RelativeNsPath}}.{{Lower.EntityName}}').columns.toArray()
            })
        );

        _createModal.onResult(function () {
            _dataTable.ajax.reload();
        });

        _editModal.onResult(function () {
            _dataTable.ajax.reload();
        });
        {{#HasRequestField}}

        _$wrapper.find("button[name=Query{{EntityName}}]").click(function (e) {
            _dataTable.ajax.reload();
        });  
        {{/HasRequestField}}
        {{#AllowBatchDelete}}

        _$wrapper.find("button[name=Delete{{EntityName}}]").click(function (e) {
            let ids = _.map(_dataTable.rows({ selected: true }).data(), function (data) {
                return data.id;
            });
            abp.message.confirm(l('ItemWillBeDeletedMessageWithFormat', ids.length + ' ' + l('Menu:{{EntityName}}')))
                .then(function (confirmed) {
                    if (confirmed) {
                        _{{Lower.EntityName}}AppService.deleteMany(ids)
                            .then(function () {
                                _dataTable.ajax.reload();
                            });
                    }
                });
        });
        _$wrapper.find("#chkSelectAll").change(function () {
            if (this.checked) {
                _dataTable.rows().select();
            } else {
                _dataTable.rows().deselect();
            }
        });
        {{/AllowBatchDelete}}

        _$wrapper.find('button[name=Create{{EntityName}}]').click(function (e) {
            e.preventDefault();
            _createModal.open();
        });
    });
{{/IsHierarcy}}
{{#IsHierarcy}}
    var _tree = new abp.tree({
        main: "#{{Lower.EntityName}}Tree",
        noItem: "#{{Lower.EntityName}}TreeEmptyInfo",
        addBtn: "button[name=Create{{EntityName}}]",
        permissions: {
            update: "{{ModuleName}}.{{EntityName}}.Update",
            create: "{{ModuleName}}.{{EntityName}}.Create",
            delete: "{{ModuleName}}.{{EntityName}}.Delete",
            move: "{{ModuleName}}.{{EntityName}}.Move",
        },
        modals: {
            createModal: _createModal,
            editModal: _editModal
        },
        services: {
            get: _{{Lower.EntityName}}AppService.getChildren,
            delete: _{{Lower.EntityName}}AppService.delete,
            move: _{{Lower.EntityName}}AppService.moveTo
        },
        localResource: "{{ModuleName}}"
    });
{{/IsHierarcy}}
})(jQuery);
