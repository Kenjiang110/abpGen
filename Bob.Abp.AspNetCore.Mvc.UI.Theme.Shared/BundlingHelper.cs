using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace Bob.Abp.AspNetCore.Mvc.UI.Theme.Shared;

public static class BundlingHelper
{
    public static AbpBundlingOptions ConfigureStyleBundles(this AbpBundlingOptions options, string styleBundleName, params string[] fileNames)
    {
        options.StyleBundles.Configure(styleBundleName,
            bundle =>
            {
                bundle.AddFiles(fileNames);
            }
        );
        return options;
    }

    public static AbpBundlingOptions ConfigureStyleBundles(this AbpBundlingOptions options, string styleBundleName, params IBundleContributor[] contributors)
    {
        options.StyleBundles.Configure(styleBundleName,
            bundle =>
            {
                bundle.AddContributors(contributors);
            }
        );
        return options;
    }

    public static AbpBundlingOptions ConfigureScriptBundles(this AbpBundlingOptions options, string scriptBundleName, params string[] fileNames)
    {
        options.ScriptBundles.Configure(scriptBundleName,
            bundle =>
            {
                bundle.AddFiles(fileNames);
            }
        );
        return options;
    }

    public static AbpBundlingOptions ConfigureScriptBundles(this AbpBundlingOptions options, string scriptBundleName, params IBundleContributor[] contributors)
    {
        options.ScriptBundles.Configure(scriptBundleName,
            bundle =>
            {
                bundle.AddContributors(contributors);
            }
        );
        return options;
    }
}
