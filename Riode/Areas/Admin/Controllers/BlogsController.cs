﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riode.Areas.Admin.Helpers.Extensions;
using Riode.Areas.Admin.ViewModels.BlogViewModel;
using Riode.Contexts;
using Riode.Models;
using Riode.ViewModels;
using System;

namespace Riode.Areas.Admin.Controllers;

[Area("Admin")]
public class BlogsController : Controller
{
    private readonly RiodeDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public BlogsController(RiodeDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        var blog = _context.Blogs.ToList();

        return View(blog);
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
    public async Task<IActionResult> Create(BlogCreateViewModel newBlog)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        
        foreach (var item in newBlog.BlogImages)
        {
            if (!item.CheckFileType("image/"))
            {
                ModelState.AddModelError("Image", "Only images are allowed");
                return View();
            }

            if (!item.CheckFileSize(3000))
            {
                ModelState.AddModelError("Image", "Image size is too big");
                return View();
            }
        }

        List<BlogImage> images = new();

        foreach (var item in newBlog.BlogImages)
        {
            string fileName = $"{Guid.NewGuid()}-{item.FileName}";
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "blog", fileName);
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await item.CopyToAsync(stream);
            }
            images.Add(new BlogImage
            {
                Image = fileName
            });
        }

        List<BlogTopic> blogTopics = new();
        foreach (var item in newBlog.BlogTopics)
        {
            Topic topic = new();
            topic.Name = item;
            blogTopics.Add(new BlogTopic
            {
                Topic = topic    
            });
        } 

        //List<BlogTopic> blogTopics = new();

        //foreach (var item in newBlog.Topic)
        //{
        //    blogTopics.Add(new BlogTopic
        //    {
        //        Topic.n = item
        //    });
        //}

        Blog blog = new()
        {
            Title = newBlog.Title,
            Description = newBlog.Description,
            Author = newBlog.Author,
            Content = newBlog.Content,
            FamousWord = newBlog.FamousWord,
            AuthorComment = newBlog.AuthorComment,
            isDeleted = false,
            Images = images,
            BlogTopics = blogTopics,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,

        };
        await _context.Blogs.AddAsync(blog);

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }


    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {

        var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id && !b.isDeleted); ;
        if (blog == null)
        {
            return NotFound();
        }
        return View(blog);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    //public async Task<IActionResult> Deleteblog(int id)
    //{

    //    var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id && !b.isDeleted); ;
    //    if (blog == null)
    //    {
    //        return NotFound();
    //    }
    //    string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "blog", blog.PosterImage);

    //    if (System.IO.File.Exists(path))
    //    {
    //        System.IO.File.Delete(path);
    //    }

    //    blog.isDeleted = true;
    //    await _context.SaveChangesAsync();
    //    return RedirectToAction("Index");
    //}
    public async Task<IActionResult> Detail(int id)
    {

        var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id && !b.isDeleted); ;
        if (blog == null)
        {
            return NotFound();
        }
        return View(blog);
    }
    //public async Task<IActionResult> Update(int id)
    //{
    //    var blog = await _context.Blogs.AsNoTracking()
    //        .FirstOrDefaultAsync(b => b.Id == id && !b.isDeleted);
    //    if (blog == null)
    //        return NotFound();

    //    BlogUpdateViewModel model = new()
    //    {
    //        Title = blog.Title,
    //        Description = blog.Description,
    //        Author = blog.Author,
    //        Content = blog.Content,
    //        FamousWord = blog.FamousWord,
    //        AuthorComment = blog.AuthorComment,
    //        CreatedDate = DateTime.UtcNow,
    //        UpdatedDate = DateTime.UtcNow,
    //    };

    //    return View(model);
    //}
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Update(int id, BlogUpdateViewModel blog)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        return View();
    //    }

    //    var updateblog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id && !b.isDeleted);
    //    if (updateblog == null)
    //    {
    //        return NotFound();
    //    }

    //    if (blog.PosterImage != null)
    //    {
    //        if (blog.PosterImage.CheckFileSize(3000))
    //        {
    //            ModelState.AddModelError("Image", "Image size is too big");
    //            return View();
    //        }

    //        if (!blog.PosterImage.CheckFileType("image/"))
    //        {
    //            ModelState.AddModelError("Image", "Only images are allowed");
    //            return View();
    //        }

    //        string basePath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "images", "blog");
    //        string path = Path.Combine(basePath, updateblog.PosterImage);

    //        if (System.IO.File.Exists(path))
    //        {
    //            System.IO.File.Delete(path);
    //        }

    //        string fileName = $"{Guid.NewGuid()}-{blog.PosterImage.FileName}";
    //        path = Path.Combine(basePath, fileName);

    //        using (FileStream stream = new FileStream(path, FileMode.Create))
    //        {
    //            await blog.PosterImage.CopyToAsync(stream);
    //        }
    //        updateblog.PosterImage = fileName;
    //    }


    //    updateblog.Title = blog.Title;
    //    updateblog.Description = blog.Description;
    //    updateblog.Author = blog.Author;
    //    updateblog.Content = blog.Content;
    //    updateblog.FamousWord = blog.FamousWord;
    //    updateblog.AuthorComment = blog.AuthorComment;
    //    updateblog.CreatedDate = DateTime.UtcNow;
    //    updateblog.UpdatedDate = DateTime.UtcNow;

    //    await _context.SaveChangesAsync();

    //    return RedirectToAction(nameof(Index));
    //}
}