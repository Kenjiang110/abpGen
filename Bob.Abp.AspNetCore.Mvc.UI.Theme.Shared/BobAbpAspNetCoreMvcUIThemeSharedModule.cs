using Bob.Abp.AspNetCore.Mvc.UI.Theme.Shared.Bundling;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Bundling;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace Bob.Abp.AspNetCore.Mvc.UI.Theme.Shared;

[DependsOn(
    typeof(AbpAspNetCoreMvcUiThemeSharedModule)
)]
public class BobAbpAspNetCoreMvcUIThemeSharedModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(BobAbpAspNetCoreMvcUIThemeSharedModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<BobAbpAspNetCoreMvcUIThemeSharedModule>("Bob.Abp.AspNetCore.Mvc.UI.Theme.Shared");
        });

        Configure<AbpBundlingOptions>(options =>
        {
            options.ConfigureStyleBundles(StandardBundles.Styles.Global, new FlagIconCssStyleContributor());
            options.ConfigureScriptBundles(StandardBundles.Scripts.Global, new BobSharedScriptContributor());
        });
    }
}
