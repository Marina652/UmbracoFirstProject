using DataAnnotationsExtensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace UmbracoFirstProject.Web.ViewModel;

public class AccountViewModel
{
    [DisplayName("Full Name")]
    [Required(ErrorMessage = "Please enter your name")]
    public string Name { get; set; }

    [DisplayName("Email")]
    [Required(ErrorMessage = "Please enter your email address")]
    [Email(ErrorMessage = "Please enter a valid email address")]
    public string Email { get; set; }

    public string? UserName { get; set; }

    [UIHint("Password")]
    [DisplayName("Password")]
    [MinLength(10, ErrorMessage = "Please make your password at least 10 characters")]
    public string? Password { get; set; }

    [UIHint("Confirm Password")]
    [DisplayName(" Confirm Password")]
    [EqualTo("Password", ErrorMessage = "Please ensure your password match")]
    public string? ConfirmPassword { get; set; }
}
