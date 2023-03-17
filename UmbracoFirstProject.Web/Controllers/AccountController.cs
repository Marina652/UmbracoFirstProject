using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.Security;
using Umbraco.Cms.Web.Website.Controllers;
using UmbracoFirstProject.Web.ViewModel;

namespace UmbracoFirstProject.Web.Controllers;

public class AccountController : SurfaceController
{
    private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/Login";
    private readonly IMemberManager _memberManager;

    public AccountController(IUmbracoContextAccessor umbracoContextAccessor,
        IUmbracoDatabaseFactory databaseFactory,
        ServiceContext services,
        AppCaches appCaches,
        IProfilingLogger profilingLogger,
        IPublishedUrlProvider publishedUrlProvider,
        IMemberManager memberManager) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
    {
        _memberManager = memberManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HandleUpdateDetails(AccountViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("Error", "There has been a problem.");
            return View("~/Views/MyAccount.cshtml");
        }

        var member = await _memberManager.GetCurrentMemberAsync();

        if (!_memberManager.IsLoggedIn() || member == null)
        {
            ModelState.AddModelError("Error", "You are not logged on.");
            return View("~/Views/MyAccount.cshtml");
        }

        member.Name = vm.Name;
        member.Email = vm.Email;
        //vm.UserName = member.UserName;

        await _memberManager.UpdateAsync(member);

        TempData["status"] = "Updated Details";

        return View("~/Views/MyAccount.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HandlePasswordChange(AccountViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("Error", "There is a problem with the form.");
      
            return View("~/Views/MyAccount.cshtml");
        }

        var member = await _memberManager.GetCurrentMemberAsync();

        if (!_memberManager.IsLoggedIn() || member == null)
        {
            ModelState.AddModelError("Error", "You are not logged on.");
            return View("~/Views/MyAccount.cshtml");
        }

        try
        {
            var token = await _memberManager.GeneratePasswordResetTokenAsync(member);
            await _memberManager.ChangePasswordWithResetAsync(member.Id, token, vm.Password);
        }
        catch(Exception ex)
        {
            ModelState.AddModelError("Error", "There's a problem with your password " + ex.Message);
            return View("~/Views/MyAccount.cshtml");
        }

        TempData["status"] = "Updated Password";

        return View("~/Views/MyAccount.cshtml");
    }
}
