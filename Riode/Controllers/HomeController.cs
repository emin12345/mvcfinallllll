using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riode.Contexts;
using Riode.ViewModels;

namespace Riode.Controllers;
public class HomeController : Controller
{
    private readonly RiodeDbContext _context;

    public HomeController(RiodeDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .Where(p => p.IsStock)
            .Where(p => !p.IsDeleted)
            .Include(p => p.Category)
            .Include(p => p.Images)
            .ToListAsync();

        var categories = await _context.Categories
            .Where(p => !p.IsDeleted)
            .ToListAsync();

        HomeViewModel model = new()
        {
            Products = products,
            Categories = categories,
            ProductImages = products.Select(p => p.Images.FirstOrDefault()).ToList()

        };
        return View(model);
    }
}
