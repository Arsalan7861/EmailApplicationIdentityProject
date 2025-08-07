using Microsoft.AspNetCore.Mvc;

namespace EmailApplicationIdentity.Controllers;

public class UserLayoutController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
