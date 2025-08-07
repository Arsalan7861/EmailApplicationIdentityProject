using EmailApplicationIdentity.Context;
using Microsoft.AspNetCore.Mvc;

namespace EmailApplicationIdentity.Controllers
{
    public class ActivationController : Controller
    {
        private readonly EmailContext _context;

        public ActivationController(EmailContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult UserActivation()
        {
            var email = TempData["Email"];
            return View(model: email);
        }

        [HttpPost]
        public IActionResult UserActivation(int userActivationCode, string email)
        {
            var code = _context.Users
                .Where(u => u.Email == email)
                .Select(u => u.ActivationCode)
                .FirstOrDefault();

            if (code == userActivationCode)
            {
                var user = _context.Users
                    .FirstOrDefault(u => u.Email == email);
                if (user != null)
                {
                    user.EmailConfirmed = true; // Mark the user as confirmed
                    _context.SaveChanges();
                }
                return RedirectToAction("UserLogin", "Login");
            }


            return View();

        }
    }
}
//nihd ugqi yiau arzd