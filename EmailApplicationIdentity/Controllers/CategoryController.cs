using EmailApplicationIdentity.Context;
using EmailApplicationIdentity.Entities;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace EmailApplicationIdentity.Controllers;
public class CategoryController : Controller
{
    private readonly EmailContext _context;

    public CategoryController(EmailContext context)
    {
        _context = context;
    }

    public IActionResult CategoryList()
    {
        var token = Request.Cookies["jwtToken"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("UserLogin", "Login");
        }

        JwtSecurityToken jwtToken;
        try
        {
            jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        }
        catch
        {
            return RedirectToAction("UserLogin", "Login");
        }

        var city = jwtToken.Claims.FirstOrDefault(c => c.Type == "City")?.Value;
        if (city?.ToLower() != "ankara")
        {
            return Forbid();
        }

        var values = _context.Categories.ToList();
        return View(values);
    }

    [HttpGet]
    public IActionResult CreateCategory()
    {
        return View();
    }
    [HttpPost]
    public IActionResult CreateCategory(Category category)
    {
        if (ModelState.IsValid)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction("CategoryList");
        }
        return View(category);
    }

    public IActionResult DeleteCategory(int id)
    {
        var category = _context.Categories.Find(id);
        if (category != null)
        {
            _context.Categories.Remove(category);
            _context.SaveChanges();
        }
        return RedirectToAction("CategoryList");
    }

    public IActionResult UpdateCategory(int id)
    {
        var category = _context.Categories.Find(id);
        if (category != null)
        {
            return View(category);
        }
        return RedirectToAction("CategoryList");
    }
    [HttpPost]
    public IActionResult UpdateCategory(Category category)
    {
        if (ModelState.IsValid)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
            return RedirectToAction("CategoryList");
        }
        return View(category);
    }
}
