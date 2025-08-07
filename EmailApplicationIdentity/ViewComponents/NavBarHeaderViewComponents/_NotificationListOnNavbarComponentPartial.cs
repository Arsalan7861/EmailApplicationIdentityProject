using EmailApplicationIdentity.Context;
using Microsoft.AspNetCore.Mvc;

namespace EmailApplicationIdentity.ViewComponents.NavBarHeaderViewComponents
{
    public class _NotificationListOnNavbarComponentPartial : ViewComponent
    {
        private readonly EmailContext _context;

        public _NotificationListOnNavbarComponentPartial(EmailContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var notifications = _context.Notifications.ToList();
            return View(notifications);
        }
    }
}
