using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models.PublishedContent;
using UmbracoFirstProject.Web.ViewModel;

namespace UmbracoFirstProject.Web.Components;

public class RenderContactForm : ViewComponent
{
    private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/";

    public IViewComponentResult Invoke(IPublishedContent siteSettings)
    {
        var vm = new ContactFormViewModel();
        
        if(siteSettings != null )
        {
            var siteKey = siteSettings.Value<string>("recaptchaSiteKey");
            vm.RecaptchaSiteKey = siteKey;
        }

        return View(PARTIAL_VIEW_FOLDER + "ContactForm.cshtml", vm);
    }
}
