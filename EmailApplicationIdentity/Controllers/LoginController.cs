using EmailApplicationIdentity.Context;
using EmailApplicationIdentity.Entities;
using EmailApplicationIdentity.Models.IdentityModels;
using EmailApplicationIdentity.Models.JwtModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmailApplicationIdentity.Controllers;

public class LoginController : Controller
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly EmailContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly JwtSettingsModel _jwtSettings;

    public LoginController(SignInManager<AppUser> signInManager, EmailContext context, UserManager<AppUser> userManager, IOptions<JwtSettingsModel> jwtSettings)
    {
        _signInManager = signInManager;
        _context = context;
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
    }

    [HttpGet]
    public IActionResult UserLogin()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UserLogin(UserLoginViewModel model)
    {
        var value = _context.Users
            .FirstOrDefault(u => u.UserName == model.Username);

        SimpleUserViewModel simpleUserViewModel = new SimpleUserViewModel
        {
            Id = value?.Id,
            Name = value?.Name,
            Surname = value?.Surname,
            City = value?.City,
            Username = value?.UserName,
            Email = value?.Email
        };

        if (value == null)
        {
            ModelState.AddModelError(string.Empty, "Kullanıcı Bulunamadı!!");
            return View(model);
        }

        if (!value.EmailConfirmed)
        {
            ModelState.AddModelError("", "Kullanıcı e-posta adresi onaylanmamış.");
            return View(model);
        }

        if (!value.IsActive)
        {
            ModelState.AddModelError("", "Kullanıcı Pasif Durumda, Giriş Yapamaz.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
        if (result.Succeeded)
        {
            if (simpleUserViewModel.City is not null)
            {
                var token = GenerateJwtToken(simpleUserViewModel);

                Response.Cookies.Append("jwtToken", token, new CookieOptions // Create a cookie to store the JWT token
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes)
                });
            }

            return RedirectToAction("Inbox", "Message");
        }

        ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
        return View(model);
    }

    public string GenerateJwtToken(SimpleUserViewModel simpleUserViewModel)
    {
        var claim = new[] {
                new Claim("Name", simpleUserViewModel.Name),
                new Claim("Surname", simpleUserViewModel.Surname),
                new Claim("City", simpleUserViewModel.City),
                new Claim("Username", simpleUserViewModel.Username),
                new Claim(ClaimTypes.NameIdentifier, simpleUserViewModel.Id),
                new Claim(ClaimTypes.Email, simpleUserViewModel.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // Create a signing key using the symmetric key and HMAC SHA256 algorithm
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claim,
            expires: DateTime.Now.AddMinutes(_jwtSettings.ExpireMinutes), // Set the token expiration time
            signingCredentials: creds // Use the signing credentials created above
        );

        return new JwtSecurityTokenHandler().WriteToken(token); // Return the view with the generated token
    }

    [HttpGet]
    public IActionResult LoginWithGoogle()
    {
        return View();
    }

    // This action method is used to initiate the external login process with Google
    [HttpPost]
    public IActionResult ExternalLogin(string provider, string? returnUrl = null)
    {
        // Redirect to the external login provider
        var redirectUrl = Url.Action("ExternalLoginCallback", "Login", new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    // This action method is called by the external login provider after the user has authenticated
    [HttpGet]
    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
    {
        returnUrl ??= Url.Content("~/");

        if (remoteError != null)
        {
            ModelState.AddModelError(string.Empty, $"External login error: {remoteError}");
            return View("UserLogin");
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            ModelState.AddModelError(string.Empty, "External login information not found.");
            return View("UserLogin");
        }

        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

        if (result.Succeeded)
        {
            return RedirectToAction("Inbox", "Message");
        }
        else
        {
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = new AppUser
            {
                Email = email,
                UserName = email,
                Name = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "Google",
                Surname = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "User",
            };

            var identityResult = await _userManager.CreateAsync(user);
            if (identityResult.Succeeded)
            {
                await _userManager.AddLoginAsync(user, info);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Inbox", "Message");
            }
            return RedirectToAction("UserLogin");
        }
    }
}
