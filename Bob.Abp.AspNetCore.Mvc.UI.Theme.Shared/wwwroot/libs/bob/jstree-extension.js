var abp = abp || {};

(function ($) {

    abp.tree = (function () {

        var defaultOptions = {
            main: ".jstree",
            noItem: ".jstreenoItem",
            addBtn: "button[name=createRootItem]",
            permissions: {  //permission consts or function(node, actionName)
                update: null,
                create: null,
                delete: null,
                move: null
            },
            modals: {
                createModal: null,
                editModal: null
            },
            services: {
                get: null,
                delete: null,
                move: null,
            },
            args: {
                get: null,   //get(params, node)
                create: null,  //crate(params, pnode)
            },
            plugins: [
                'types',
                'contextmenu',
                'wholerow',
                'sort',
                'dnd'  //drap and drop
            ],
            localResource: null, //function or resource name
            contextMenu: null,
            contextMenuSelector: '.ou-text .fa-caret-down',
            maps: {
                item2node: null, //function(node, item)
                item2order: null,  //function(item)
                item2text: null,  //function(item)
                item2opened: null,  //function(item)
                item2icon: null,  //function(item)
            },
        };

        //node: mixed, node object (check node.parent) or node.parent directly.
        function _node2ItemParentId(node) {
            if (!node || !node.parent || node.parent == "#" || node == "#") {
                return null;
            } else if (node.parent) {
                return node.parent;
            } else {
                return node;
            }
        }

        function _checkPermission(permission, node, action) {
            if (permission && typeof (permission) == "string") {
                return abp.auth.isGranted(permission);
            } else if (permission && typeof (permission) == "function") {
                return permission(node, action);
            } else {
                return false;
            }
        }

        function _getLocalization(localization) {
            if (localization && typeof (localization) == "string") {
                return abp.localization.getResource(localization);
            } else if (localization && typeof (localization) == "function") {
                return localization;
            } else {
                return abp.localization.getResource();
            }
        }

        var _tree = function (options) {
            var _options = {};
            if (options) {
                $.extend(true, _options, defaultOptions, options);
            } else {
                $.extend(true, _options, defaultOptions);
            }

            var l = _getLocalization(_options.localResource);
            if (!_options.contextMenu) _options.contextMenu = _contextMenu;

            var _$tree = $(_options.main);
            var _$emptyInfo = $(_options.noItem);
            var _actionNode = null;
            var _itemCount = 0;

            function _setItemCount(itemCount) {
                _itemCount = itemCount;
                if (_itemCount) {
                    if (_$emptyInfo) _$emptyInfo.hide();
                    if (_$tree) _$tree.show();
                } else {
                    if (_$emptyInfo) _$emptyInfo.show();
                    if (_$tree) _$tree.hide();
                }
            }

            function _refreshItemCount() {
                //get whole tree data to determine the tree's item count
                var itemCount = _$tree.jstree('get_json').length;
                _setItemCount(itemCount);
            }

            function _refresh(node) {
                _$tree.jstree('refresh_node', node);
                _refreshItemCount();
            }

            function _rollback(moveData) {
                _$tree.off('move_node.jstree', null, _nodeMovedHandler);  //prevent trigger recursively.
                //move node from old_parent-old_position to parent-positon
                _$tree.jstree('move_node', moveData.node, moveData.old_parent, moveData.old_position);
                _$tree.on('move_node.jstree', _nodeMovedHandler); //restore
            }

            function _pnode2CreateArgs(pnode) { //create child
                var args = pnode ?
                    { parentId: pnode.id, parentCode: pnode.original.code, parentName: pnode.original.name, parentDisplayName: pnode.original.displayName }
                    : { parentId: null, parentCode: null, parentName: null, parentDisplayName: null };

                return _options.args.create ? _options.args.create(args, pnode) : args;
            }

            //contextMenu action function's argument (menuItem) is information about the context menu item.
            //var instance = $.jstree.reference(menuItem.reference); find out the tree selected.
            function _contextMenu(node) {
                _actionNode = node;  //record the node

                var _editItem = {
                    label: l('Edit'),
                    icon: 'fa fa-pencil',
                    _disabled: !_checkPermission(_options.permissions.update, node, "Update"),
                    action: function (menuItem) {
                        if (_options.modals.editModal) {
                            _options.modals.editModal.open({ id: node.id });
                        } else {
                            alert('No edit modal.')
                        }
                    }
                };

                var _addSubItem = {
                    label: l('AddSubItem'),
                    icon: 'fa fa-plus',
                    _disabled: !_checkPermission(_options.permissions.create, node, "Create"),
                    action: function (menuItem) {
                        if (_options.modals.createModal) {
                            var params = _pnode2CreateArgs(node);
                            _options.modals.createModal.open(params);
                        } else {
                            alert('No create modal.')
                        }
                    }
                };

                var _delItem = {
                    label: l('Delete'),
                    icon: 'fa fa-remove',
                    _disabled: !_checkPermission(_options.permissions.delete, node, "Delete"),
                    action: function (menuItem) {
                        if (_options.services.delete) {
                            var instance = $.jstree.reference(menuItem.reference);
                            abp.message.confirm(
                                l('ItemWillBeDeletedMessageWithFormat', node.original.displayName),
                                l('AreYouSure'),
                                function (isConfirmed) {
                                    if (isConfirmed) {
                                        _options.services.delete(node.id)
                                            .done(function () {
                                                instance.delete_node(node);
                                                _refreshItemCount();
                                            })
                                            .fail(function (err) {
                                                setTimeout(function () {
                                                    abp.message.error(err.message);
                                                }, 500);
                                            });
                                    }
                                }
                            )
                        } else {
                            alert('No delete service.')
                        }
                    }
                };

                var _refreshItem = {
                    label: l('Refresh'),
                    icon: 'fa fa-sync',
                    action: function (menuItem) {
                        _refresh(node);
                    }
                };

                return {
                    edit: _editItem,
                    addSubItem: _addSubItem,
                    'delete': _delItem,
                    'refresh': _refreshItem
                }
            }

            function _item2Text(item) {
                var itemClass = ' ou-text-no-members';
                var title = _options.maps.item2text ? _options.maps.item2text(item) : item.code || item.name;
                return '<span title="' + title + '" class="ou-text text-dark' + itemClass + '" data-item-id="' + item.id + '">' +
                    $.fn.dataTable.render.text().display(item.displayName) +
                    ' <i class="fa fa-caret-down text-muted"></i></span > ';
            }

            function _item2Icon(item) {
                return _options.maps.item2icon ? _options.maps.item2icon(item) : item.icon || 'fa fa-folder'
            }

            function _item2Opened(item) {
                if (_options.maps.item2opened) {
                    return _options.maps.item2opened(item);
                } else if (item.opened == null || item.opened == undefined) {
                    return item.children ? true : false;
                } else {
                    return item.opened;
                }
            }

            //item.id*, item.displayName*, item.code*, item.icon, item.text, item.opened, item.children
            function _item2Node(item) {
                var node = {
                    id: item.id,
                    displayName: item.displayName,
                    text: _item2Text(item),
                    icon: _item2Icon(item),
                    state: { opened: _item2Opened(item) },
                    children: item.children ? _.map(item.children, _item2Node) : true,

                    code: item.code,
                    name: item.name,
                    order: _options.maps.item2order ? _options.maps.item2order(item) : item.order
                };

                return _options.maps.item2node ? _options.maps.item2node(node, item) : node;
            }

            function _addChildAfterServerAdded(parent, item) {
                if (parent && !parent.state.opened) {
                    if (!parent.children || parent.children.length == 0) {
                        _$tree.jstree('refresh_node', parent);
                    }
                    _$tree.jstree('open_node', parent);  //open will retrive children from server
                } else {
                    _$tree.jstree('create_node', parent, _item2Node(item));
                }
                _refreshItemCount();
            }

            if (_options.modals.createModal) {
                _options.modals.createModal.onResult(function (e, data) {
                    _addChildAfterServerAdded(_actionNode, data.responseText);
                });

                var addBtn = _options.addBtn && typeof (_options.addBtn) == "string" ? $(_options.addBtn) : _options.addBtn;
                if (addBtn) {
                    addBtn.click(function (e) {  //create root item
                        e.preventDefault();
                        _actionNode = null; //root item's parent is null.
                        var params = _pnode2CreateArgs(null);
                        _options.modals.createModal.open(params);
                    });
                }
            }

            function _updateNode(node, item) {
                node.original.text = _item2Text(item);
                _$tree.jstree('rename_node', node, node.original.text);

                node.original.icon = _item2Icon(item);
                _$tree.jstree('set_icon', node, node.original.icon);

                node.original.displayName = item.displayName;
            }

            if (_options.modals.editModal) {
                _options.modals.editModal.onResult(function (e, data) {
                    _updateNode(_actionNode, data.responseText);
                });
            }

            _$tree.on('click', _options.contextMenuSelector, function (e) {  //show context menu when "contextMenuSelector" was clicked.
                e.preventDefault();
                var id = $(this).closest('.ou-text').attr('data-item-id');
                setTimeout(function () {
                    _$tree.jstree('show_contextmenu', id);
                }, 100);
            });

            _$tree.on('delete_node.jstree', function (e, data) {
                e.preventDefault();
                _refreshItemCount();
            });

            _$tree.on('changed.jstree', function (e, data) {  //selected node changed event handler
                if (data.selected.length != 1) {
                    _actionNode = null;
                } else {
                    var selectedNode = data.instance.get_node(data.selected[0]);
                    _actionNode = selectedNode;
                }
            });

            function _nodeMovedHandler(e, data) {
                if (data.old_parent == data.parent) {
                    return;
                } else if (!_options.services.move) {
                    alert("No move service.");
                } else if (_checkPermission(_options.permissions.move, data.node, "Move")) {
                    var parentNodeName = (!data.parent || data.parent == '#')
                        ? l('Root')
                        : _$tree.jstree('get_node', data.parent).original.displayName;

                    abp.message.confirm(
                        l('ItemMoveConfirmMessage', data.node.original.displayName, parentNodeName),
                        l('AreYouSure'),
                        function (isConfirmed) {
                            if (isConfirmed) {
                                var moveRequest = { id: data.node.id, newParentId: _node2ItemParentId(data) };
                                _options.services.move(moveRequest).done(function () {
                                    //_refresh(data.parent); 
                                }).fail(function (err) {
                                    _rollback(data);
                                    setTimeout(function () {
                                        abp.message.error(err.message);
                                    }, 500);
                                });
                            } else {
                                _rollback(data);
                            }
                        }
                    );
                } else {
                    _rollback(data);
                }
            };
            _$tree.on('move_node.jstree', _nodeMovedHandler);

            function _pnode2GetArgs(pnode) {  //get children
                var args = { parentId: pnode.id === "#" ? null : pnode.id };

                return _options.args.get ? _options.args.get(args, pnode) : args;
            }

            _$tree.jstree({
                'core': {
                    data: function (node, callback) {
                        if (_options.services.get) {
                            var params = _pnode2GetArgs(node);
                            _options.services.get(params).done(function (result) {  //get data from service
                                var data = _.map(result.items || result, _item2Node);  //"_" is lodash, //result.items if paged list.
                                callback(data);
                                if (!params.parentId && data && data.length > 0) {  //for initial showing state
                                    _setItemCount(data.length);
                                }
                            });
                        } else {
                            alert("No get service.");
                        }
                    },
                    multiple: false,
                    check_callback: function (operation, node, node_parent, node_position, more) {
                        return true;  //return true to allow the action on the tree.
                    }
                },

                contextmenu: {
                    items: _options.contextMenu,
                    'select_node': true
                },

                sort: function (node1, node2) {
                    if (this.get_node(node2).original.order < this.get_node(node1).original.order) {
                        return 1;
                    }
                    return -1;
                },

                plugins: _options.plugins
            });

            var _publicApi = {  //expose slef
                options: _options,  //readonly
                $tree: _$tree,
                rollback: _rollback,
                refreshItemCount: _refreshItemCount,
                refresh: _refresh,
                updateNode: _updateNode
            };
            return _publicApi;
        };

        return _tree;  //_tree is a function which return _publicApi object.
    })();  //abp.tree = _tree

})(jQuery);