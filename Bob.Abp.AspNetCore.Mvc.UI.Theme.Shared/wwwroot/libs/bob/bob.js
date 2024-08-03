(function ($) {
    abp.utils.clearFile = function ($file, onchange) {
        //clear
        var $new = $file.clone();
        $new.val("");
        $file.after($new);
        $file.remove();

        if (onchange) $new.change(onchange);
        return $new;
    }

    abp.utils.isWeChat = function () {
        return /MicroMessenger/i.test(window.navigator.userAgent);
    }

    /**
     * use this function to change action column of datatables (change ABP module's datatable appearence), for example:
     * 
     * 1. Add file "role-extensions.js" to "Pages/IdentityService/Identity/Roles"
     * (function ($) {
     *      abp.ui.extensions.tableColumns.get('identity.role').addContributor(
     *          function(columnList) { //function as a column contributor
     *              abp.libs.datatables.tailActionColumn(columnList, 
     *                  true,   //to move action column to end. 
     *                  true    //use button group instead of dropdown buttons. 
     *          }         
     *     );
     * })(jQuery);
     * 
     * 2. Add AbpBundlingOptions to WebModule's ConfigureServices()
     * 
     *      Configure<AbpBundlingOptions>(options => {
     *           options.ConfigureScriptBundles(typeof(Volo.Abp.Identity.Web.Pages.Identity.Roles.IndexModel).FullName,
     *               "/Pages/IdentityService/Identity/Roles/role-extensions.js");
     *      });   
     * 
     *  @param { boolean } tailAction : If true, it will move action column to the end of the datatable.
     *  @param { boolean } useButtonGroup : If true, it will use button group for actions.
     */
    abp.libs.datatables.setActionColumn = function (columnList, tailAction, useButtonGroup) {
        var o = columnList.find(node => node.value.rowAction);
        if (o) {
            if (tailAction) {
                columnList.detach(o);
                columnList.addTail(o.value);
            }
            if (useButtonGroup) {
                o.value.rowAction.useButtonGroup = true;
            }
        }
        return o;
    }

    if (!$.fn.dataTable) {
        return;
    }

    /**
     * get visibility of the filed.
     * @param {boolean | function} visibilityField boolean or function(record, tableInstance)
     * @param {any} record data of the row
     * @param {any} tableInstance the dataTable instance
     */
    var getVisibilityValue = function (visibilityField, record, tableInstance) {
        if (visibilityField === undefined) {
            return true;
        }

        if (abp.utils.isFunction(visibilityField)) {
            return visibilityField(record, tableInstance);
        } else {
            return visibilityField;
        }
    };

    /**
     * add icon for $target
     *   @param fieldItem providing icon
     *      icon: name of icon，the icon class is "fa fa-[icon]"
     *      iconClass: icon class, for example: "fa fa-lg"
     *   @param defIconClass default icon class, for example: "fa fa-cog"
     */
    var _setIcon = function ($target, fieldItem, defIconClass) {
        var iconClass = defIconClass;
        if (fieldItem.icon !== undefined && fieldItem.icon) {  //has icon info
            iconClass = "fa fa-" + fieldItem.icon + " mr-1";
        } else if (fieldItem.iconClass) {  //has icon class
            iconClass = fieldItem.iconClass + " mr-1";
        }

        if (iconClass) {
            $target.append($("<i>").addClass(iconClass));
        }
        return iconClass;
    };

    /**
     * excute fieldItem.action
     * @param {any} record data of the row
     * @param {any} fieldItem column item
     *      fieldItem.confirmMessage: string or function to get confirm dialog's text, call abp.message.confirm() before call the action if provided
     *      fieldItem.action : fieldItem.action()
     * @param {any} tableInstance the dataTable instance
     */
    var _fireAction = function (record, fieldItem, tableInstance) {
        if (fieldItem.confirmMessage) {  //should use confirmMessage to construct the confirm prompt information?
            abp.message.confirm(fieldItem.confirmMessage({ record: record, table: tableInstance }))
                .done(function (accepted) {
                    if (accepted) {
                        fieldItem.action({ record: record, table: tableInstance });  //excute fieldItem.action if confirmed.
                    }
                });
        } else {
            fieldItem.action({ record: record, table: tableInstance });  //excute fieldItem.action directly.
        }
    };

    /**
     * Try to get icon class for ABP module
     * @param {any} title of the action item
     * @returns icon class or undefined
     */
    var _fromTitleToIcon = function (title) {
        //AbpIdentity、AbpTenantManagement
        var moduleName;
        if ($("#IdentityUsersWrapper").length > 0 || $("#IdentityRolesWrapper").length > 0) {
            moduleName = "AbpIdentity";
        } else if ($("#TenantsWrapper").length > 0) {
            moduleName = "AbpTenantManagement";
        }
        var _l = abp.localization.getResource(moduleName);
        if (_l('Edit') == title) {
            return "fa fa-edit";
        } else if (_l('Features') == title) {
            return "fa fa-cubes";
        } else if (_l('Delete') == title) {
            return "fa fa-trash-alt";
        } else if (_l('Permissions') == title) {
            return "fa fa-key";
        }

        return;
    }

    /**
     * Create a button for button group.
     * @param {any} record data of the row
     * @param {any} fieldItem column item
     *      fieldItem.class : button's class, default value is "btn-primary"
     *      fieldItem.text : tip for the button and first 2 letters of the button as icon if icon is not set
     *      fieldItem.action : bind action to the button
     * @param {any} tableInstance the dataTable instance
     */
    var _createButton = function (record, fieldItem, tableInstance) {
        var $btn = $('<button type="button" class="btn"/>');
        if (fieldItem.class) {
            $btn.addClass(fieldItem.class);
        } else {
            $btn.addClass("btn-primary");
        }
        //set icon
        if (!_setIcon($btn, fieldItem, _fromTitleToIcon(fieldItem.text))) {
            if (fieldItem.text) {
                $btn.text(fieldItem.text.substring(0, 2));
            }
        }

        if (fieldItem.text) {
            $btn.attr("title", fieldItem.text);  //text prompt
        }

        if (fieldItem.action) {
            $btn.click(function (e) {
                e.preventDefault();

                if (!$(this).hasClass('disabled')) {
                    _fireAction(record, fieldItem, tableInstance);
                }
            });
        }

        return $btn;
    };

    /**
     * Create buttons (group) for the column
     * @param {any} record data of the row
     * @param {any} field column info
        *      field.items : action items of the column
     * @param {any} tableInstance the dataTable instance
     * @returns
     */
    var _createButtonGroup = function (record, field, tableInstance) {
        var $container = $('<div/>')
            .addClass('btn-group btn-group-sm')
            .addClass('abp-action-button');  //same as abp dropdown buttons

        for (var i = 0; i < field.items.length; i++) {
            var fieldItem = field.items[i];
            if (!getVisibilityValue(fieldItem.visible, record)) {  //visible = false
                continue;
            }

            var $button = _createButton(record, fieldItem, tableInstance);
            if (fieldItem.enabled && !fieldItem.enabled({ record: record })) {  //disabled?
                $button.addClass('disabled');
            }

            $button.appendTo($container);
        }

        if ($container.children().length === 0) {
            return "";
        }

        return $container;
    };

    var renderRowActions = function (tableInstance, nRow, aData, iDisplayIndex, iDisplayIndexFull) {
        var columns; //all cloumns of this row

        if (tableInstance.aoColumns) {
            columns = tableInstance.aoColumns;
        } else {
            columns = tableInstance.fnSettings().aoColumns;
        }

        if (!columns) {
            return;
        }

        var cells = $(nRow).children("td");  //all <td> of this row in the table

        for (var colIndex = 0; colIndex < columns.length; colIndex++) {
            var column = columns[colIndex];
            if (column.rowAction && column.rowAction.useButtonGroup) {  //if this is a row action column and we want to use button group
                var $actionContainer = _createButtonGroup(aData, column.rowAction, tableInstance);   //button group
                if ($actionContainer) {
                    $(cells[colIndex]).empty().append($actionContainer);  //replace to button group
                }
            }
        }
    };

    //set fnRowCallback to use ButtonGroup instead of dropdown buttons
    var _existingFnRowCallback = $.fn.dataTable.defaults.fnRowCallback;
    $.extend(true, $.fn.dataTable.defaults, {
        fnRowCallback: function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {             
            if (_existingFnRowCallback) {
                _existingFnRowCallback.call(this, nRow, aData, iDisplayIndex, iDisplayIndexFull);  //call saved old fnRowCallback 
            }

            renderRowActions(this, nRow, aData, iDisplayIndex, iDisplayIndexFull);  //my renderer to show row action
        }
    });
})(jQuery);