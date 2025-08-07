using EmailApplicationIdentity.Context;
using Microsoft.AspNetCore.Mvc;

namespace EmailApplicationIdentity.ViewComponents.MesssageViewComponents
{
    public class _MessageCategoryListSideBarComponentPartial: ViewComponent
    {
        private readonly EmailContext _emailContext;

        public _MessageCategoryListSideBarComponentPartial(EmailContext emailContext)
        {
            _emailContext = emailContext;
        }

        public IViewComponentResult Invoke()
        {
            var categories = _emailContext.Categories.ToList();
            return View(categories);
        }
    }
}
