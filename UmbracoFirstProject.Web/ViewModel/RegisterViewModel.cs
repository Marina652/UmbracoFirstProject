using DataAnnotationsExtensions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UmbracoFirstProject.Web.ViewModel;

public class RegisterViewModel
{
    [DisplayName("First Name")]
    [Required(ErrorMessage = "Please enter your first name")]
    public string FirstName { get; set; }

    [DisplayName("Last Name")]
    [Required(ErrorMessage = "Please enter your last name")]
    public string LastName { get; set; }

    [DisplayName("User Name")]
    [Required(ErrorMessage = "Please enter your user name")]
    [MinLength(6)]
    public string UserName { get; set; }

    [DisplayName("Email")]
    [Required(ErrorMessage = "Please enter your email")]
    public string EmailAddress { get; set; }

    [UIHint("Password")]
    [DisplayName("Password")]
    [Required(ErrorMessage = "Please enter your password")]
    [MinLength(10, ErrorMessage = "Please make your password at least 10 characters")]
    public string Password { get; set; }

    [UIHint("Confirm Password")]
    [DisplayName(" Confirm Password")]
    [Required(ErrorMessage = "Please enter your password confirmation")]
    [EqualTo("Password", ErrorMessage = "Please ensure your password match")]
    public string ConfirmPassword { get; set; }
}
