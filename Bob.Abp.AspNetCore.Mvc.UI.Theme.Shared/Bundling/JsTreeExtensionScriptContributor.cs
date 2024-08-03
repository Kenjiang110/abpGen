using System.Collections.Generic;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Packages.JsTree;
using Volo.Abp.Modularity;

namespace Bob.Abp.AspNetCore.Mvc.UI.Theme.Shared.Bundling;

[DependsOn(typeof(JsTreeScriptContributor))]
public class JsTreeExtensionScriptContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        context.Files.AddIfNotContains("/libs/bob/jstree-extension.js");
    }
}
