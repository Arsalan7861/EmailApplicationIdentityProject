using EmailApplicationIdentity.Context;
using EmailApplicationIdentity.Entities;
using EmailApplicationIdentity.Models.MessageViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EmailApplicationIdentity.ViewComponents.NavBarHeaderViewComponents
{
    public class _MessageListOnNavBarHeaderComponentPartial : ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly EmailContext _context;

        public _MessageListOnNavBarHeaderComponentPartial(UserManager<AppUser> userManager, EmailContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var messages = _context.Messages
                .Where(x => x.ReceiverEmail == user.Email && !x.IsRead)
                .Select(x => new MessageListWithUserInfoViewModel
                {
                    FullName = $"{ user.Name } {user.Surname}",
                    ProfileImageUrl = user.ImageUrl,
                    MessageDetail = x.MesssageDetail,
                    SendDate = x.SendDate
                })
                .ToList();
            return View(messages);
        }
    }
}
