using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common;
using Umbraco.Cms.Web.Website.Controllers;
using UmbracoFirstProject.Web.ViewModel;
using System.Net.Mail;
using Newtonsoft.Json.Linq;

namespace UmbracoFirstProject.Web.Controllers;

public class ContactController : SurfaceController
{
    private const string PARTIAL_VIEW_FOLDER = "~/Views/Partials/";

    private readonly UmbracoHelper _umbracoHelper;

    private readonly IConfiguration _configuration;

    public ContactController(IUmbracoContextAccessor umbracoContextAccessor,
        IUmbracoDatabaseFactory databaseFactory,
        ServiceContext services,
        AppCaches appCaches,
        IProfilingLogger profilingLogger,
        IPublishedUrlProvider publishedUrlProvider,
        UmbracoHelper umbracoHelper,
        IConfiguration configuration) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
    {
        _umbracoHelper = umbracoHelper;
        _configuration = configuration;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult HandleContactForm(ContactFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("Error", "Please check the form.");

            return Redirect("https://localhost:44312/ru/about/");
            //return View(PARTIAL_VIEW_FOLDER + "ContactForm.cshtml", vm);
        }

        var siteSettings = _umbracoHelper.ContentAtRoot().DescendantsOrSelfOfType("siteSettings").FirstOrDefault();
        if(siteSettings != null)
        {
            var secretKey = siteSettings.Value<string>("recaptchaSecretKey");
            var isCaptchaValid = IsCaptchaValid(Request.Form["GoogleCaptchaToken"], secretKey);
            if (!isCaptchaValid)
            {
                ModelState.AddModelError("Captcha", "The captcha is not valid are you human?");
                return Redirect("https://localhost:44312/ru/about/");
            }
        }

        try
        {
            var contactForms = _umbracoHelper.ContentAtRoot().DescendantsOrSelfOfType("contactForms").FirstOrDefault();
            if (contactForms != null)
            {
                var newContact = Services.ContentService.Create("Contact", contactForms.Id, "contactForm");
                newContact.SetValue("contactName", vm.Name);
                newContact.SetValue("contactEmail", vm.EmailAddress);
                newContact.SetValue("contactSubject", vm.Subject);
                newContact.SetValue("contactComments", vm.Comment);

                Services.ContentService.SaveAndPublish(newContact);
            }

            SendContactFormReceivedEmail(vm);

            TempData["status"] = "OK";

            return Redirect("https://localhost:44312/about/");
        }
        catch (Exception ex) 
        {
            //Logger.Error("There was an error in the contact form submission");
            ModelState.AddModelError("Error", "Sorry there was a problem noting your details. Would you please try again later?");
        }
       
        return Redirect("https://localhost:44312/about/");
    }

    private bool IsCaptchaValid(string token, string secretKey)
    {
        // Sending the token to google api
        HttpClient httpClient = new HttpClient();

        var res = httpClient
            .GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}")
            .Result;

        if(res.StatusCode != System.Net.HttpStatusCode.OK)
            return false;

        // get response
        string jsonRes = res.Content.ReadAsStringAsync().Result;
        dynamic jsonData = JObject.Parse(jsonRes);
        if(jsonData.success != true)
        {
            return false;
        }

        return true;
        // was good?
    }

    private void SendContactFormReceivedEmail(ContactFormViewModel vm)
    {
        // Read email FROM and TO addresses
        // Get site settings
        var siteSettings = _umbracoHelper.ContentAtRoot().DescendantsOrSelfOfType("siteSettings").FirstOrDefault();
        if (siteSettings == null) 
        {
            throw new Exception("There are no site settings");
        }

        var fromAddress = siteSettings.Value<string>("emailSettingsFromAddress");
        var toAddresses = siteSettings.Value<string>("emailSettingsAdminAccounts");

        if (string.IsNullOrEmpty(fromAddress))
        {
            throw new Exception("There needs to be a from address in site settings");
        }

        if (string.IsNullOrEmpty(toAddresses))
        {
            throw new Exception("There needs to be a to address in site settings");
        }

        // Construct the actual email
        var emailSubject = "There has been a contact form submitted";
        var emailBody = $"A new contact form has been received from {vm.Name}. Their comments were: {vm.Comment}";
        var smtpMessage = new MailMessage();
        smtpMessage.Subject = emailSubject;
        smtpMessage.Body = emailBody;
        smtpMessage.From = new MailAddress(fromAddress);

        var toList = toAddresses.Split(',');
        foreach(var item in toList)
        {
            if (!string.IsNullOrEmpty(item))
                smtpMessage.To.Add(item);
        }

        // Send via whatever email service
        using(var smtp = new SmtpClient())
        {
            smtp.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            smtp.Host = "localhost";
            smtp.PickupDirectoryLocation = "d:\\Umbraco\\mail";

            smtp.Send(smtpMessage);
        }
    }
}
