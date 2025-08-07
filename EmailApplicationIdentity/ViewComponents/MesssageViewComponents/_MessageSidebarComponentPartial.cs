using EmailApplicationIdentity.Context;
using EmailApplicationIdentity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EmailApplicationIdentity.ViewComponents.MesssageViewComponents
{
    public class _MessageSidebarComponentPartial: ViewComponent
    {
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _userManager;

        public _MessageSidebarComponentPartial(EmailContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            ViewBag.sendMessageCount = _context.Messages.Where(m => m.SenderEmail == user.Email).Count();
            ViewBag.receiveMessageCount = _context.Messages.Where(m => m.ReceiverEmail == user.Email).Count();
            return View();
        }
    }
}
