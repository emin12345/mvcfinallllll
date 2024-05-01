using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Riode.Contexts;
using Riode.Models;
using Riode.ViewModels;

namespace Riode.ViewComponents;

public class ProductViewComponent : ViewComponent
{
    private readonly RiodeDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    public ProductViewComponent(RiodeDbContext context, UserManager<AppUser> userManager = null)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var products = await _context.Products
            .Where(p => !p.IsDeleted)
            .Where(p => p.IsStock)
            .Include(p => p.Category)
            .Take(6).ToListAsync();

        var productImages = await _context.ProductImages.ToListAsync();

        var user = await _userManager.FindByNameAsync(User.Identity.Name);

     

        var productImageViewModel = new ProductImageViewModel
        {
            Products = products,
            ProductImages = productImages,
            
        };

        return View(productImageViewModel);
    }
}