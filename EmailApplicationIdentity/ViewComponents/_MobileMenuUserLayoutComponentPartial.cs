using Microsoft.AspNetCore.Mvc;

namespace EmailApplicationIdentity.ViewComponents
{
    public class _MobileMenuUserLayoutComponentPartial: ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
