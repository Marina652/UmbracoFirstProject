using System.Web.Mvc;
using UmbracoFirstProject.Web.ViewModel;
using Umbraco.Cms.Web.Website.Controllers;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace UmbracoFirstProject.Web.Components;

public class Register : ViewComponent
{
	private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/";

	public IViewComponentResult Invoke()
	{
		var vm = new RegisterViewModel();
		return View(PARTIAL_VIEW_FOLDER + "Register.cshtml", vm);
	}
}
