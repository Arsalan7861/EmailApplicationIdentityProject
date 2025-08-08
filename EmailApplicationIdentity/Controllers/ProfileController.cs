using EmailApplicationIdentity.Entities;
using EmailApplicationIdentity.Models.IdentityModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EmailApplicationIdentity.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<AppUser> _userManager;

    public ProfileController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        UserEditViewModel userEditViewModel = new UserEditViewModel
        {
            Username = user.UserName,
            Name = user.Name,
            Surname = user.Surname,
            City = user.City,
            ImageUrl = user.ImageUrl,
            PhoneNumber = user.PhoneNumber,
            Email = user.Email
        };
        return View(userEditViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditProfile(UserEditViewModel model)
    {
        if (ModelState.IsValid)
        {
            return View(model);
        }
        if (!model.Email.Contains("@") || !model.Email.EndsWith(".com"))
        {
            ModelState.AddModelError("Email", "Lütfen geçerli bir e-posta adresi girin.");
            return View(model);
        }
        if (model.Password == null && model.PasswordConfirm == null)
        {
            ModelState.AddModelError("", "Şifre alanları boş bırakılamaz.");
            return View(model);
        }

        if (model.Password == model.PasswordConfirm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            user.UserName = model.Username;
            user.Name = model.Name;
            user.Surname = model.Surname;
            user.City = model.City;
            user.PhoneNumber = model.PhoneNumber;
            user.Email = model.Email;
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.Password);
            var result = await _userManager.UpdateAsync(user);
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
            }
        }
        else
        {
            ModelState.AddModelError("", "Passwords do not match.");
        }
        return View(model);

    }
}
