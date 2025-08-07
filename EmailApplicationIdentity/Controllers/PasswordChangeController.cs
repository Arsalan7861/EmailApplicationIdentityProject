using EmailApplicationIdentity.Entities;
using EmailApplicationIdentity.Models.ForgetPasswordViewModels;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace EmailApplicationIdentity.Controllers
{
    public class PasswordChangeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public PasswordChangeController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel forgetPasswordViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(forgetPasswordViewModel);
            }

            if(!forgetPasswordViewModel.Email.Contains("@") || !forgetPasswordViewModel.Email.EndsWith(".com"))
            {
                ModelState.AddModelError("Email", "Email Should cntain '@' and end with '.com'");
                return View(forgetPasswordViewModel);
            }

            var user = await _userManager.FindByEmailAsync(forgetPasswordViewModel.Email);
            if (user == null)
            {
                // Do not reveal that the user does not exist
                return View("ForgetPasswordConfirmation");
            }
            string resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetPasswordLink = Url.Action("ResetPassword", "PasswordChange", new { userId = user.Id, token = resetPasswordToken }, HttpContext.Request.Scheme);

            MimeMessage mimeMessage = new MimeMessage();
            MailboxAddress mailboxAddressFrom = new MailboxAddress("Admin", "admin@gmail.com"); // Sender email address
            mimeMessage.From.Add(mailboxAddressFrom);

            MailboxAddress mailboxAddressTo = new MailboxAddress(user.Name + " " + user.Surname, user.Email); // Recipient email address
            mimeMessage.To.Add(mailboxAddressTo);

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = $"To reset your password, please click the following link: {resetPasswordLink}";
            mimeMessage.Body = bodyBuilder.ToMessageBody();
            mimeMessage.Subject = "Password Reset Request";
            SmtpClient smtpClient = new SmtpClient();
            await smtpClient.ConnectAsync("smtp.gmail.com", 587, false);
            await smtpClient.AuthenticateAsync("balikcilik.fishing@gmail.com", "nihd ugqi yiau arzd"); // Use your actual email and password here
            await smtpClient.SendAsync(mimeMessage);
            await smtpClient.DisconnectAsync(true);
            return View("ForgetPasswordConfirmation");
        }

        [HttpGet]
        public IActionResult ForgetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid password reset link.");
            }
            var model = new ResetPasswordViewModel
            {
                UserId = userId,
                Token = token
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            if (resetPasswordViewModel.NewPassword != resetPasswordViewModel.ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View(resetPasswordViewModel);
            }
            var user = await _userManager.FindByIdAsync(resetPasswordViewModel.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            var result = await _userManager.ResetPasswordAsync(user, resetPasswordViewModel.Token, resetPasswordViewModel.NewPassword);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(resetPasswordViewModel);
            }
        }

        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            if (changePasswordViewModel.NewPassword != changePasswordViewModel.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "New password and confirmation do not match.");
                return View(changePasswordViewModel);
            }
            var result = await _userManager.ChangePasswordAsync(user, changePasswordViewModel.CurrentPassword, changePasswordViewModel.NewPassword);
            if (result.Succeeded)
            {
                return RedirectToAction("UserLogin", "Login");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(changePasswordViewModel);
            }
        }
    }
}
