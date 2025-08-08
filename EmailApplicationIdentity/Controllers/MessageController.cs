using EmailApplicationIdentity.Context;
using EmailApplicationIdentity.Entities;
using EmailApplicationIdentity.Models.MessageViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmailApplicationIdentity.Controllers;

public class MessageController : Controller
{
    private readonly EmailContext _context;
    private readonly UserManager<AppUser> _userManager;

    public MessageController(EmailContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Inbox()
    {
        var user = await _userManager.FindByNameAsync(User.Identity.Name);

        var messages = (from m in _context.Messages
                        join u in _context.Users
                        on m.SenderEmail equals u.Email into userGroup
                        from sender in userGroup.DefaultIfEmpty()

                        join c in _context.Categories
                        on m.CategoryId equals c.CategoryId into categoryGroup
                        from category in categoryGroup.DefaultIfEmpty()

                        where m.ReceiverEmail == user.Email
                        select new MessageWithSenderInfoViewModel
                        {
                            MessageId = m.MessageId,
                            Subject = m.Subject,
                            MessageDetail = m.MesssageDetail,
                            SendDate = m.SendDate,
                            SenderEmail = m.SenderEmail,
                            SenderName = sender != null ? sender.Name : "Unknown",
                            SenderSurname = sender != null ? sender.Surname : "User",
                            CategoryName = category != null ? category.CategoryName : "No Category"
                        }).ToList();

        return View(messages);
    }

    public async Task<IActionResult> Sendbox()
    {
        var user = await _userManager.FindByNameAsync(User.Identity.Name);

        var messages = (from m in _context.Messages
                        join u in _context.Users
                        on m.ReceiverEmail equals u.Email into userGroup
                        from receiver in userGroup.DefaultIfEmpty()

                        join c in _context.Categories
                        on m.CategoryId equals c.CategoryId into categoryGroup
                        from category in categoryGroup.DefaultIfEmpty()

                        where m.SenderEmail == user.Email
                        select new MessageWithRecieverViewModel
                        {
                            MessageId = m.MessageId,
                            Subject = m.Subject,
                            MessageDetail = m.MesssageDetail,
                            SendDate = m.SendDate,
                            RecieverEmail = m.SenderEmail,
                            RecieverName = receiver != null ? receiver.Name : "Unknown",
                            RecieverSurname = receiver != null ? receiver.Surname : "User",
                            CategoryName = category != null ? category.CategoryName : "No Category"
                        }).ToList();

        return View(messages);
    }

    public async Task<IActionResult> MessageDetail(int id)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Invalid message ID.");
            return RedirectToAction("Inbox");
        }

        var value = _context.Messages.Where(x => x.MessageId == id).FirstOrDefault();
        value!.IsRead = true; // Mark the message as read
        await _context.SaveChangesAsync();
        return View(value);
    }

    [HttpGet]
    public IActionResult ComposeMessage()
    {
        var categories = _context.Categories.ToList();
        ViewBag.c = categories.Select(c => new SelectListItem
        {
            Text = c.CategoryName, // for display
            Value = c.CategoryId.ToString() // for backend processing
        }).ToList();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ComposeMessage(Message message)
    {
        if (!ModelState.IsValid || message.CategoryId == 0)
        {
            ModelState.AddModelError("", "Please fill in all required fields.");
            var categories = _context.Categories.ToList();
            ViewBag.c = categories.Select(c => new SelectListItem
            {
                Text = c.CategoryName,
                Value = c.CategoryId.ToString()
            }).ToList();
            return View(message);
        }

        var user = await _userManager.FindByNameAsync(User.Identity.Name);

        message.SenderEmail = user.Email;
        message.SendDate = DateTime.Now;
        message.IsRead = false;

        _context.Messages.Add(message);
        _context.SaveChanges();
        return RedirectToAction("Sendbox");

    }

    public async Task<IActionResult> MessageCategory(int id)
    {
        if (id is 0)
        {
            return RedirectToAction("Inbox");
        }
        var user = await _userManager.FindByNameAsync(User.Identity.Name);

        if (user == null)
        {
            return RedirectToAction("UserLogin", "Login");
        }


        var messages = (from m in _context.Messages
                        join u in _context.Users
                        on m.SenderEmail equals u.Email into userGroup
                        from sender in userGroup.DefaultIfEmpty()

                        join c in _context.Categories
                        on m.CategoryId equals c.CategoryId into categoryGroup
                        from category in categoryGroup.DefaultIfEmpty()

                        where m.CategoryId == id
                        where m.ReceiverEmail == user.Email
                        select new MessageWithSenderInfoViewModel
                        {
                            MessageId = m.MessageId,
                            Subject = m.Subject,
                            MessageDetail = m.MesssageDetail,
                            SendDate = m.SendDate,
                            SenderEmail = m.SenderEmail!,
                            SenderName = sender != null ? sender.Name : "Unknown",
                            SenderSurname = sender != null ? sender.Surname : "User",
                            CategoryName = category != null ? category.CategoryName : "No Category"
                        }).ToList();
        return View(messages);
    }


    public async Task<IActionResult> DeleteMessage(int id)
    {
        if (id == 0)
        {
            return RedirectToAction("Inbox");
        }
        var message = await _context.Messages.FindAsync(id);
        if (message == null)
        {
            return NotFound();
        }
        _context.Messages.Remove(message);
        await _context.SaveChangesAsync();
        return RedirectToAction("Inbox");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSelectedMessagesInbox(List<int> selectedIds)
    {
        if (selectedIds == null || !selectedIds.Any())
        {
            return RedirectToAction("Inbox");
        }

        var messagesToDelete = _context.Messages.Where(m => selectedIds.Contains(m.MessageId));
        _context.Messages.RemoveRange(messagesToDelete);
        await _context.SaveChangesAsync();

        return RedirectToAction("Inbox");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSelectedMessagesSendbox(List<int> selectedIds)
    {
        if (selectedIds == null || !selectedIds.Any())
        {
            return RedirectToAction("Sendbox");
        }

        var messagesToDelete = _context.Messages.Where(m => selectedIds.Contains(m.MessageId));
        _context.Messages.RemoveRange(messagesToDelete);
        await _context.SaveChangesAsync();

        return RedirectToAction("Sendbox");
    }

    [HttpPost]
    public async Task<IActionResult> SearchEmailAsync(string searchTerm)
    {
        var user = await _userManager.FindByNameAsync(User.Identity.Name);

        if (string.IsNullOrEmpty(searchTerm))
        {
            return RedirectToAction("Inbox");
        }
        var messages = (from m in _context.Messages
                        join u in _context.Users
                        on m.SenderEmail equals u.Email into userGroup
                        from sender in userGroup.DefaultIfEmpty()

                        join c in _context.Categories
                        on m.CategoryId equals c.CategoryId into categoryGroup
                        from category in categoryGroup.DefaultIfEmpty()

                        where m.ReceiverEmail == user.Email &&
                              (m.Subject.ToLower().Contains(searchTerm.ToLower()) || m.MesssageDetail.ToLower().Contains(searchTerm.ToLower()) || m.SenderEmail.ToLower().Contains(searchTerm.ToLower()))
                        select new MessageWithSenderInfoViewModel
                        {
                            MessageId = m.MessageId,
                            Subject = m.Subject,
                            MessageDetail = m.MesssageDetail,
                            SendDate = m.SendDate,
                            SenderEmail = m.SenderEmail,
                            SenderName = sender != null ? sender.Name : "Unknown",
                            SenderSurname = sender != null ? sender.Surname : "User",
                            CategoryName = category != null ? category.CategoryName : "No Category"
                        }).ToList();
        return View("Inbox", messages);
    }
}
