using EmailApplicationIdentity.Context;
using EmailApplicationIdentity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EmailApplicationIdentity.Controllers;

public class CommentsController : Controller
{
    private readonly EmailContext _context;
    private readonly UserManager<AppUser> _userManager;

    public CommentsController(EmailContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult UserComments()
    {
        var comments = _context.Comments.Include(x => x.AppUser).ToList();

        return View(comments);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult UserCommentList()
    {
        var comments = _context.Comments.Include(x => x.AppUser).ToList();
        return View(comments);
    }

    public PartialViewResult CreateComment()
    {
        return PartialView();
    }

    [HttpPost]
    public async Task<IActionResult> CreateComment(Comment comment)
    {
        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        comment.AppUserId = user.Id;
        comment.CommentDate = DateTime.Now;

        // Toxic Bert Api Analysis
        using (var client = new HttpClient())
        {
            var apiKey = "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"; // Replace with your actual Hugging Face API key
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);


            try
            {
                var translateRequestBody = new
                {
                    inputs = comment.CommentDetail
                };

                var translateJson = JsonSerializer.Serialize(translateRequestBody);
                var translateContent = new StringContent(translateJson, Encoding.UTF8, "application/json");
                var translateResponse = await client.PostAsync("https://api-inference.huggingface.co/models/Helsinki-NLP/opus-mt-tr-en", translateContent);
                var translateResponseString = await translateResponse.Content.ReadAsStringAsync();
                string englishComment = comment.CommentDetail;
                if (translateResponseString.TrimStart().StartsWith("["))
                {
                    var translateDoc = JsonDocument.Parse(translateResponseString);
                    englishComment = translateDoc.RootElement[0].GetProperty("translation_text").GetString();
                }

                var toxicRequestBody = new
                {
                    inputs = englishComment
                };

                var toxicJson = JsonSerializer.Serialize(toxicRequestBody);
                var toxicContent = new StringContent(toxicJson, Encoding.UTF8, "application/json");

                var toxicResponse = await client.PostAsync("https://api-inference.huggingface.co/models/unitary/toxic-bert", toxicContent);
                var toxicResponseString = await toxicResponse.Content.ReadAsStringAsync();

                if (toxicResponseString.TrimStart().StartsWith("["))
                {
                    var toxicDoc = JsonDocument.Parse(toxicResponseString);
                    foreach (var item in toxicDoc.RootElement[0].EnumerateArray())
                    {
                        var label = item.GetProperty("label").GetString();
                        var score = item.GetProperty("score").GetSingle();

                        if (score > 0.5)
                        {
                            comment.CommentStatus = "Toxic Comment";
                            break;
                        }
                    }
                }
                if (string.IsNullOrEmpty(comment.CommentStatus))
                {
                    comment.CommentStatus = "Comment Approved";
                }
            }
            catch (Exception)
            {
                comment.CommentStatus = "Waiting Approval";
            }
        }

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        return RedirectToAction("UserCommentList");
    }

    public IActionResult DeleteComment(int id)
    {
        var comment = _context.Comments.Find(id);
        if (comment != null)
        {
            _context.Comments.Remove(comment);
            _context.SaveChanges();
        }
        return RedirectToAction("UserCommentList");
    }

    public IActionResult ToxicComment(int id)
    {
        var comment = _context.Comments.Find(id);
        if (comment != null)
        {
            comment.CommentStatus = "Toxic Comment";
            _context.SaveChanges();
        }
        return RedirectToAction("UserCommentList");
    }

    public IActionResult PassiveComment(int id)
    {
        var comment = _context.Comments.Find(id);
        if (comment != null)
        {
            comment.CommentStatus = "Comment is removed";
            _context.SaveChanges();
        }
        return RedirectToAction("UserCommentList");
    }

    public IActionResult ApproveComment(int id)
    {
        var comment = _context.Comments.Find(id);
        if (comment != null)
        {
            comment.CommentStatus = "Comment Approved";
            _context.SaveChanges();
        }
        return RedirectToAction("UserCommentList");
    }
}
