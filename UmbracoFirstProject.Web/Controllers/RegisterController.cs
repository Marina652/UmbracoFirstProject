using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;
using UmbracoFirstProject.Web.ViewModel;

namespace UmbracoFirstProject.Web.Controllers;

public class RegisterController : SurfaceController
{
    private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/";
    private readonly IMemberManager _memberManager;

    public RegisterController(IUmbracoContextAccessor umbracoContextAccessor,
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
    public async Task<IActionResult> HandleRegister(RegisterViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Register.cshtml");
        }

        var existingMember = await _memberManager.FindByEmailAsync(vm.EmailAddress);

        if(existingMember != null)
        {
            ModelState.AddModelError("Account Error", "There's already a user with that email address.");
            return View("~/Views/Register.cshtml");
        }

        existingMember = await _memberManager.FindByNameAsync(vm.UserName);


        if (existingMember != null)
        {
            ModelState.AddModelError("Account Error", "There's already a user with that email username. Please choose a different one.");
            return View("~/Views/Register.cshtml");
        }

        var identityMember = MemberIdentityUser.CreateNew(vm.UserName, vm.EmailAddress, "", true, vm.FirstName + " " + vm.LastName);

        var identityResult = await _memberManager.CreateAsync(identityMember, vm.Password);

        if (identityResult.Succeeded)
        {
            TempData["Success"] = true;

            await _memberManager.AddToRolesAsync(identityMember, new string[] { "Normal User" });
        }
        else
        {
            TempData["Success"] = false;

            var errors = identityResult.Errors;

            var errorString = new StringBuilder();

            //added for learning purposes. This could be logged
            foreach (var error in errors)
            {
                errorString.Append($"{error.Code}-{error.Description}");
            }
        }

        return Redirect("https://localhost:44312/");
    }
}
