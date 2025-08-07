using Microsoft.AspNetCore.Mvc;

namespace EmailApplicationIdentity.ViewComponents
{
    public class _HeadUserLayoutComponentPartial: ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
