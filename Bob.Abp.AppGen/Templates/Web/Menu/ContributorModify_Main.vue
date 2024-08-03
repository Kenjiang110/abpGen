
    private Task Configure{{ModuleName}}MenuAsync(MenuConfigurationContext context)
    {
        var {{ToCamel ModuleName}}MenuItem = context.Menu.GetMenuItem({{ModuleName}}Menus.Home);
        var l = context.GetLocalizer<{{ModuleName}}Resource>();


        return Task.CompletedTask;
    }
