using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg.Sig;
using Riode.Areas.Admin.Helpers.Extensions;
using Riode.Areas.Admin.ViewModels.ProductViewModels;
using Riode.Contexts;
using Riode.Models;
using Riode.ViewModels;
using System;
using System.IO;
using System.Xml.Linq;

namespace Riode.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,Moderator")]
public class ProductController : Controller
{
    private readonly RiodeDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductController(RiodeDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Brand)
            .Where(p => !p.IsDeleted)
            .ToListAsync();

        return View(products);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _context.Categories.ToListAsync();
        ViewBag.Brands = await _context.Brands.ToListAsync();


        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductCreateViewModel product)
    {
        ViewBag.Categories = await _context.Categories.ToListAsync();
        ViewBag.Brands = await _context.Brands.ToListAsync();

        if (!ModelState.IsValid)
        {
            return View();
        }
        //if (!product.ProductImage.Any(,))
        //{
        //    ModelState.AddModelError("Image", "Add an image");
        //}

        foreach (var image in product.ProductImage)
        {
            if (!image.CheckFileSize(3000))
            {
                ModelState.AddModelError("Image", "File size must be smaller than 3mb");
                return View();
            }

            if (!image.CheckFileType("image/"))
            {
                ModelState.AddModelError("Image", "Add an image format");
                return View();
            }
        }

        List<ProductImage> productImages = new();
        foreach (var image in product.ProductImage)
        {
            string fileName = $"{Guid.NewGuid()}-{image.FileName}";
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "shop", fileName);
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            ProductImage productImage = new()
            {
                Image = fileName,
                ProductId = product.Id
            };

            productImages.Add(productImage);
        }




        Product newProduct = new()
        {
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Discount = product.Discount,
            Rating = product.Rating,
            IsStock = product.IsStock,
            Images = productImages,
            Material = product.Material,
            Features = product.Features,
            RecommendedUse = product.RecommendedUse,
            IsDeleted = false,
            CategoryId = product.CategoryId,
            BrandId = product.BrandId,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        await _context.Products.AddAsync(newProduct);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Detail(int id)
    {
        var products = await _context.Products.AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (products == null)
        {
            return NotFound();
        }
        return View(products);

    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (product == null)
            return NotFound();

        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName(nameof(Delete))]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (product == null)
            return NotFound();

        foreach (var image in product.Images)
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "shop", image.Image);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        product.IsDeleted = true;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Update(int id)
    {
        var product = await _context.Products
            .Include(p => p.Images)
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (product == null)
            return NotFound();

        ProductUpdateViewModel productUpdateViewModel = new()
        {
            Name = product.Name,
            Price = product.Price,
            OldPrice = product.OldPrice,
            Rating = product.Rating,
            Description = product.Description,
            Discount = product.Discount,
            Features = product.Features,
            Material = product.Material,
            IsStock = product.IsStock,
            RecommendedUse = product.RecommendedUse,
            BrandId = product.BrandId,
            CategoryId = product.CategoryId,
            ProductImages = product.Images
        };

        ViewBag.Brands = await _context.Brands.ToListAsync();
        ViewBag.Categories = await _context.Categories.ToListAsync();

        return View(productUpdateViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, ProductUpdateViewModel productUpdateViewModel)
    {
        ViewBag.Categories = await _context.Categories.ToListAsync();
        ViewBag.Brands = await _context.Brands.ToListAsync();

        if (!ModelState.IsValid)
            return View();

        var product = await _context.Products
            .Include(p => p.Images)
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (product == null)
            return NotFound();


        List<ProductImage> ProductImages = new();
        foreach (var image in product.Images)
        {
            ProductImages.Add(image);
        }
        if (productUpdateViewModel.ProductImage != null)
        {
            foreach (var image in productUpdateViewModel.ProductImage)
            {
                if (!image.CheckFileSize(3000))
                {
                    ModelState.AddModelError("Image", "File size must be smaller than 3mb");
                    return View();
                }

                if (!image.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Image", "Add an image format");
                    return View();
                }
                string fileName = $"{Guid.NewGuid()}-{image.FileName}";
                string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "shop", fileName);
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                ProductImage productImage = new()
                {
                    Image = fileName,
                    ProductId = product.Id
                };

                ProductImages.Add(productImage);
            }
        }

        product.Name = productUpdateViewModel.Name;
        product.Price = productUpdateViewModel.Price;
        product.OldPrice = productUpdateViewModel.OldPrice;
        product.Rating = productUpdateViewModel.Rating;
        product.Description = productUpdateViewModel.Description;
        product.Discount = productUpdateViewModel.Discount;
        product.Features = productUpdateViewModel.Features;
        product.Material = productUpdateViewModel.Material;
        product.IsStock = productUpdateViewModel.IsStock;
        product.RecommendedUse = productUpdateViewModel.RecommendedUse;
        product.UpdatedDate = DateTime.UtcNow;
        product.Images = ProductImages;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> DeleteImage(int id)
    {
        var image = await _context.ProductImages.FirstOrDefaultAsync(i => i.Id == id);

        if (image == null)
            return NotFound();

        string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "shop", image.Image);
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }

        _context.ProductImages.Remove(image);
        await _context.SaveChangesAsync();

        return Json(new { message = "Image has been deleted." });
    }
}