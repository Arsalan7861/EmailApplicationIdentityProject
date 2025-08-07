using Microsoft.AspNetCore.Mvc;

namespace EmailApplicationIdentity.ViewComponents
{
    public class _FooterUserLayoutComponentPartial: ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
