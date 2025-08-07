using Microsoft.AspNetCore.Mvc;

namespace EmailApplicationIdentity.ViewComponents
{
    public class _BreadCombUserLayoutComponentPartial: ViewComponent
    {
        public IViewComponentResult Invoke(string title = "Main", string content = "Welcome", string icon = "notika-mail")
        {
            ViewBag.Title = title;
            ViewBag.Content = content;
            ViewBag.Icon = icon;
            return View();
        }
    }
}
