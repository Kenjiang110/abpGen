
    private Task Configure{{ModuleName}}MenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name != StandardMenus.Main) return Task.CompletedTask;
        var l = context.GetLocalizer<{{ModuleName}}Resource>();
        var {{ToCamel ModuleName}}MenuItem = context.Menu.GetMenuItem({{ModuleName}}Menus.Home);

        return Task.CompletedTask;
    }
