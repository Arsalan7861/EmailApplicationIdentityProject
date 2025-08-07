using EmailApplicationIdentity.Entities;
using EmailApplicationIdentity.Models.IdentityModels;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace EmailApplicationIdentity.Controllers;

public class RegisterController : Controller
{
    private readonly UserManager<AppUser> _userManager;

    public RegisterController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Signup()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Signup(RegisterUserViewModel model)
    {
        //if (!ModelState.IsValid)
        //{
        //    return View(model);
        //}

        //var existingUser = await _userManager.FindByEmailAsync(model.Email);
        //if (existingUser != null)
        //{
        //    ModelState.AddModelError(nameof(model.Email), $"'{model.Email}' e-posta adresi zaten alınmış.");
        //    return View(model);
        //}

        Random random = new Random();
        int code = random.Next(100000, 1000000);


        var user = new AppUser
        {
            UserName = model.Username,
            Email = model.Email,
            Name = model.Name,
            Surname = model.Surname,
            ActivationCode = code,
            IsActive = true
        };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            // send an email with the activation code -> nihd ugqi yiau arzd
            MimeMessage mimeMessage = new MimeMessage();
            MailboxAddress mailboxAddressFrom = new MailboxAddress("Admin", "admin@gmail.com"); // Sender email address

            mimeMessage.From.Add(mailboxAddressFrom);

            MailboxAddress mailboxAddressTo = new MailboxAddress(model.Name + " " + model.Surname, model.Email); // Recipient email address
            mimeMessage.To.Add(mailboxAddressTo);

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = $"Your activation code is: {code}";
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            mimeMessage.Subject = "Notika Activation Code";

            SmtpClient smtpClient = new SmtpClient();
            await smtpClient.ConnectAsync("smtp.gmail.com", 587, false);
            await smtpClient.AuthenticateAsync("balikcilik.fishing@gmail.com", "nihd ugqi yiau arzd"); // Use your actual email and password here
            await smtpClient.SendAsync(mimeMessage);
            await smtpClient.DisconnectAsync(true);

            TempData["Email"] = model.Email;// Store the email in TempData to use it in other pages

            return RedirectToAction("UserActivation", "Activation");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View();
    }
}
