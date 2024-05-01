using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riode.Areas.Admin.Helpers.Extensions;
using Riode.Areas.Admin.ViewModels.BrandViewModels;
using Riode.Contexts;
using Riode.Models;
using System;

namespace Riode.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Admin,Moderator")]
public class BrandController : Controller
{
    private readonly RiodeDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public BrandController(RiodeDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        var brand = _context.Brands.ToList();

        return View(brand);
    }
    public async Task<IActionResult> Create()
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BrandCreateViewModel brand)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        if (!brand.Logo.CheckFileSize(3000))
        {
            ModelState.AddModelError("Logo", "Too Big!");
            return View();
        }
        if (!brand.Logo.CheckFileType("image/"))
        {
            ModelState.AddModelError("Logo", "Only image formats are allowed");
            return View();
        }
        string fileName = $"{Guid.NewGuid()}-{brand.Logo.FileName}";
        string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "brands", fileName);
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            await brand.Logo.CopyToAsync(stream);
        }
        Brand newbrand = new()
        {
            Name = brand.Name,
            Logo = fileName,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
        };
        await _context.Brands.AddAsync(newbrand);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {

        var brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted); ;
        if (brand == null)
        {
            return NotFound();
        }
        return View(brand);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBrand(int id)
    {

        var brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted); ;
        if (brand == null)
        {
            return NotFound();
        }
        string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "brands", brand.Logo);

        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }

        brand.IsDeleted = true;
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
    public async Task<IActionResult> Detail(int id)
    {

        var brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted); ;
        if (brand == null)
        {
            return NotFound();
        }
        return View(brand);
    }
    public async Task<IActionResult> Update(int id)
    {
        var brand = await _context.Brands.AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
        if (brand == null)
            return NotFound();

        BrandUpdateViewModel model = new()
        {
            Name = brand.Name,
        };

        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, BrandUpdateViewModel brand)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        var updateBrand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
        if (updateBrand == null)
        {
            return NotFound();
        }

        if (brand.Logo != null)
        {
            if (!brand.Logo.CheckFileSize(3000))
            {
                ModelState.AddModelError("Logo", "Logo size must be smaller than 3mb");
                return View();
            }

            if (!brand.Logo.CheckFileType("image/"))
            {
                ModelState.AddModelError("Logo", "Only image formats are allowed");
                return View();
            }

            string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "brands", updateBrand.Logo);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            string fileName = $"{Guid.NewGuid()}-{brand.Logo.FileName}";
            path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "brands", fileName);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await brand.Logo.CopyToAsync(stream);
            }
            updateBrand.Logo = fileName;
        }


        updateBrand.Name = brand.Name;
        updateBrand.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}