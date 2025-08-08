using EmailApplicationIdentity.Models.JwtModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmailApplicationIdentity.Controllers;

public class TokenController : Controller
{
    private readonly JwtSettingsModel _jwtSettings;

    public TokenController(IOptions<JwtSettingsModel> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value; // Inject JwtSettingsModel using IOptions - this allows us to access the JWT settings from the configuration
    }

    [HttpGet]
    public IActionResult Generate()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Generate(SimpleUserViewModel simpleUserViewModel)
    {
        if (simpleUserViewModel.Name is null)
        {
            ModelState.AddModelError(string.Empty, "Ad eksik.");
            return View(simpleUserViewModel);
        }
        if (simpleUserViewModel.Surname is null)
        {
            ModelState.AddModelError(string.Empty, "Soyad eksik.");
            return View(simpleUserViewModel);
        }
        if (simpleUserViewModel.City is null)
        {
            ModelState.AddModelError(string.Empty, "Şehir adı eksik.");
            return View(simpleUserViewModel);
        }
        if (simpleUserViewModel.Username is null)
        {
            ModelState.AddModelError(string.Empty, "Kullanıcı adı eksik.");
            return View(simpleUserViewModel);
        }
        var claim = new[] {
            new Claim("Name", simpleUserViewModel.Name),
            new Claim("Surname", simpleUserViewModel.Surname),
            new Claim("City", simpleUserViewModel.City),
            new Claim("Username", simpleUserViewModel.Username),
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

        simpleUserViewModel.Token = new JwtSecurityTokenHandler().WriteToken(token); // Generate the token string
        return View(simpleUserViewModel); // Return the view with the generated token
    }
}
