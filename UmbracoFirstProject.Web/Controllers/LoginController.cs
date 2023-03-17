using Microsoft.AspNetCore.Mvc;
using Namotion.Reflection;
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

public class LoginController : SurfaceController
{
    private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/Login";
    private readonly IMemberManager _memberManager;
    private readonly IMemberSignInManager _memberSignInManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoginController(IUmbracoContextAccessor umbracoContextAccessor,
        IUmbracoDatabaseFactory databaseFactory,
        ServiceContext services,
        AppCaches appCaches,
        IProfilingLogger profilingLogger,
        IPublishedUrlProvider publishedUrlProvider,
        IMemberManager memberManager,
        IMemberSignInManager memberSignInManager,
        IHttpContextAccessor httpContextAccessor) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
    {
        _memberManager = memberManager;
        _memberSignInManager = memberSignInManager;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HandleLogin(LoginViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Login.cshtml");
        }

        var member = await _memberManager.FindByNameAsync(vm.UserName);


        if (member == null)
        {
            ModelState.AddModelError("Login", "Cannot find that username in the system");
            return View("~/Views/Login.cshtml");
        }

        if (member.IsLockedOut)
        {
            ModelState.AddModelError("Login", "The account is locked, please use forgotten password to reset");
            return View("~/Views/Login.cshtml");
        }

        var emailVerified = member.TryGetPropertyValue<bool>("emailVerified");
        // !!!!
        emailVerified = true;
        if (!emailVerified)
        {
            ModelState.AddModelError("Login", "Please verified your email before loging in");
            return View("~/Views/Login.cshtml");
        }

        var validCredentials = await _memberManager.ValidateCredentialsAsync(vm.UserName, vm.Password);
        if (!validCredentials)
        {
            ModelState.AddModelError("Login", "The username/password your provided is not correct");
            return View("~/Views/Login.cshtml");
        }

        if (!string.IsNullOrEmpty(vm.RedirectUrl))
        {
            return Redirect(vm.RedirectUrl);
        }

        var loginResult = await _memberSignInManager.PasswordSignInAsync(vm.UserName, vm.Password, true, true);
        return View("~/Views/Login.cshtml");
    }
}
