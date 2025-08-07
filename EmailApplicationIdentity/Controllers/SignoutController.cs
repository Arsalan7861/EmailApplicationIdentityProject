using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace EmailApplicationIdentity.Controllers
{
    public class SignoutController : Controller
    {
        public IActionResult Signout()
        {
            // Sign out the user
            HttpContext.SignOutAsync();
            // Clear the authentication cookie
            HttpContext.SignOutAsync("Identity.Application");
           
            return RedirectToAction("UserLogin", "Login");
        }
    }
}
