using Microsoft.AspNetCore.Mvc;

namespace EmailApplicationIdentity.Controllers
{
    public class ErrorPageController : Controller
    {
        [Route("Error/404")]
        public IActionResult Page404()
        {
            return View();
        }

        [Route("Error/401")]
        public IActionResult Pag401()
        {
            return View();
        }
        [Route("Error/403")]
        public IActionResult Pag403()
        {
            return View();
        }

        [Route("Error/{statusCode}")]
        public IActionResult HandleError(int statusCode)
        {
            if (statusCode == 404)
            {
                return RedirectToAction("Page404");
            }
            if (statusCode == 401)
            {
                return RedirectToAction("Page401");
            }
            if (statusCode == 403)
            {
                return RedirectToAction("Page403");
            }
            return View();
        }
    }
}
