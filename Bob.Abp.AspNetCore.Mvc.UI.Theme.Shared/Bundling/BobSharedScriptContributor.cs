using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Packages.JQueryValidationUnobtrusive;
using Volo.Abp.Modularity;

namespace Bob.Abp.AspNetCore.Mvc.UI.Theme.Shared.Bundling
{
    [DependsOn(
        typeof(JQueryValidationUnobtrusiveScriptContributor)
        )]
    public class BobSharedScriptContributor : BundleContributor
    {
        public override void ConfigureBundle(BundleConfigurationContext context)
        {
            context.Files.Add("/libs/bob/jqvalidation.extension.js");
            context.Files.Add("/libs/bob/bob.js");
            //put datatables-extensions.js after bob.js
            //context.Files.Remove("/libs/abp/aspnetcore-mvc-ui-theme-shared/datatables/datatables-extensions.js");  
            //context.Files.Add("/libs/abp/aspnetcore-mvc-ui-theme-shared/datatables/datatables-extensions.js");
        }
    }
}
