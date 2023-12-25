using Microsoft.Extensions.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.UI.Navigation;

namespace {{RootNamespace}}.{{ModuleName}}.Web.Menus;

public class MenuItemInfo
{
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string? Url { get; set; }

    public string? Icon { get; set; }

    public string? PermissionName { get; set; }

    public MenuItemInfo(string name, string displayName, string? permissionName = null, string? url = null)
    {
        Name = name;
        DisplayName = displayName;
        Url = url;
        PermissionName = permissionName;
        Icon = string.IsNullOrEmpty(url) ? "fa fa-folder" : "fa fa-file";
    }

    public ApplicationMenuItem CreateApplicationMenuItem(IStringLocalizer l, int order = 1000)
    {
        var item = new ApplicationMenuItem(Name, l[DisplayName], Url, Icon, order);
        if (!string.IsNullOrEmpty(PermissionName))
        {
            item.RequirePermissions(PermissionName);
        }
        return item;
    }
}
