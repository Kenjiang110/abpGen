using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Validation;

namespace {{RootNamespace}}.{{ModuleName}}.Web.Pages.{{LastEntityGroup.RelativeNsPath}}.{{EntityName}};

public class IndexModel : {{ModuleName}}PageModel
{
    public virtual Task<IActionResult> OnGetAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }

    public virtual Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }
}