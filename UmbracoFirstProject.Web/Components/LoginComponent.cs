using Microsoft.AspNetCore.Mvc;
using UmbracoFirstProject.Web.ViewModel;

namespace UmbracoFirstProject.Web.Components;

public class Login : ViewComponent
{
	private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/Login/";

	public IViewComponentResult Invoke()
	{
		var vm = new LoginViewModel();
	
		return View(PARTIAL_VIEW_FOLDER + "Login.cshtml", vm);
	}
}
