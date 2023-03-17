using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Security;
using UmbracoFirstProject.Web.ViewModel;

namespace UmbracoFirstProject.Web.Components
{
	public class RenderMyAccount : ViewComponent
	{
		private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/MyAccount";

		public async Task<IViewComponentResult> InvokeAsync(IMemberManager memberManager, IHttpContextAccessor httpContextAccessor)
		{
			var vm = new AccountViewModel();

			if (!memberManager.IsLoggedIn())
			{
				ModelState.AddModelError("Error", "You are not currently logged in.");

				return Content("You are not currently logged in.");
			}

			var member = await memberManager.GetCurrentMemberAsync();
			if(member == null)
			{
				ModelState.AddModelError("Error", "We could not find you in the system.");

				return Content("We could not find you in the system.");
			}

			vm.Name = member.Name;
			vm.Email = member.Email;
			vm.UserName = member.UserName;

			return View(PARTIAL_VIEW_FOLDER + "/MyAccount.cshtml", vm);
		}
	}
}
