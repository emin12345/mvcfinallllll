using Microsoft.AspNetCore.Mvc;
using Riode.Contexts;
using Riode.Models;

namespace Riode.Controllers;
public class ContactController : Controller
{
    private readonly RiodeDbContext _context;

    public ContactController(RiodeDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Index(ContactComment newComment)
    {
        if (!ModelState.IsValid)
            return View();

        ContactComment Comment = new()
        {
            Name = newComment.Name,
            Comment = newComment.Comment,
            Email = newComment.Email,
        };
        await _context.ContactComments.AddAsync(Comment);
        await _context.SaveChangesAsync();

        if (ModelState.IsValid)
        {
            TempData["Sent"] = "Your comment was sent successfully";
            return RedirectToAction("Index");
        }

        return View();
    }
}
