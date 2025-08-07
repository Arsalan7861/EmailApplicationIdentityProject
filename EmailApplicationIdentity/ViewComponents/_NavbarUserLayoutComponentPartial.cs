using Microsoft.AspNetCore.Mvc;

namespace EmailApplicationIdentity.ViewComponents
{
    public class _NavbarUserLayoutComponentPartial: ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
