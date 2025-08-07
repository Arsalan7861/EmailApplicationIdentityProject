using EmailApplicationIdentity.Context;
using EmailApplicationIdentity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EmailApplicationIdentity.ViewComponents;

public class _HeaderUseLayoutComponentPartial : ViewComponent
{
    private readonly EmailContext _context;
    private readonly UserManager<AppUser> _userManager;

    public _HeaderUseLayoutComponentPartial(EmailContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        if (user == null)
        {
            return View();
        }
        var emailCount = _context.Messages.Count(x => x.ReceiverEmail == user.Email && !x.IsRead);
        ViewBag.EmailCount = emailCount;
        var notificationCount = _context.Notifications.Count();
        ViewBag.NotificationCount = notificationCount;
        return View();
    }
}
