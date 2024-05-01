using Riode.Contexts;
using Riode.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riode.Areas.Admin.ViewModels.CategoryViewModels;
using Riode.Areas.Admin.Helpers.Extensions;

namespace Riode.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Admin,Moderator")]
public class CategoryController : Controller
{
    private readonly RiodeDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public CategoryController(RiodeDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        var category = _context.Categories.ToList();

        return View(category);
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
    public async Task<IActionResult> Create(CategoryCreateViewModel category)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        if (!category.Image.CheckFileType("image/"))
        {
            ModelState.AddModelError("Image", "Only image formats are allowed");
            return View();
        }
        if (!category.Image.CheckFileSize(3000))
        {
            ModelState.AddModelError("Image", "Image size must be smaller than 3mb");
            return View();
        }
        string fileName = $"{Guid.NewGuid()}-{category.Image.FileName}";
        string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "categories", fileName);
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            await category.Image.CopyToAsync(stream);
        }
        Category newcategory = new()
        {
            Name = category.Name,
            Image = fileName,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
        };
        await _context.Categories.AddAsync(newcategory);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {

        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (category == null)
        {
            return NotFound();
        }
        return View(category);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCategory(int id)
    {

        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (category == null)
        {
            return NotFound();
        }
        string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "categories", category.Image);

        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }

        category.IsDeleted = true;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Detail(int id)
    {

        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (category == null)
        {
            return NotFound();
        }
        return View(category);
    }
    public async Task<IActionResult> Update(int id)
    {
        var category = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (category == null)
            return NotFound();

        CategoryUpdateViewModel model = new()
        {
            Name = category.Name,
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, CategoryUpdateViewModel categoryUpdateViewModel)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (category == null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            return View();
        }

        if (categoryUpdateViewModel.CategoryImage != null)
        {
            if (!categoryUpdateViewModel.CategoryImage.CheckFileType("image/"))
            {
                ModelState.AddModelError("Image", "Only images are allowed");
                return View();
            }

            if (!categoryUpdateViewModel.CategoryImage.CheckFileSize(3000))
            {
                ModelState.AddModelError("Image", "Image size is too big");
                return View();
            }

            string basePath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "categories");
            string path = Path.Combine(basePath, category.Image);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            string fileName = $"{Guid.NewGuid()}-{categoryUpdateViewModel.CategoryImage.FileName}";
            path = Path.Combine(basePath, fileName);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await categoryUpdateViewModel.CategoryImage.CopyToAsync(stream);
            }
            category.Image = fileName;
        }


        category.Name = categoryUpdateViewModel.Name;
        category.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}