using System.ComponentModel.DataAnnotations;

namespace UmbracoFirstProject.Web.ViewModel;

public class LoginViewModel
{
	[Display(Name = "Username")]
	[Required]
	public string UserName { get; set; }

	[Display(Name = "Password")]
	[Required]
	public string Password { get; set; }

	public string? RedirectUrl { get; set; }
}
