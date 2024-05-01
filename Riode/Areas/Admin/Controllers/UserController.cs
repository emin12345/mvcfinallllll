using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riode.Areas.Admin.ViewModels.UserViewModels;
using Riode.Contexts;
using Riode.Models;
using System;

namespace Riode.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,Moderator")]
public class UsersController : Controller
{
	private readonly RiodeDbContext _context;
	private readonly IWebHostEnvironment _webHostEnvironment;
	private readonly RoleManager<IdentityRole> _roleManager;
	private readonly UserManager<AppUser> _userManager;

	public UsersController(RiodeDbContext context, IWebHostEnvironment webHostEnvironment, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
	{
		_context = context;
		_webHostEnvironment = webHostEnvironment;
		_roleManager = roleManager;
		_userManager = userManager;
	}


	public async Task<IActionResult> Index()
	{
		var users = await _userManager.Users.ToListAsync();
		var roles = await _roleManager.Roles.ToListAsync();
		var UserRoles = new List<UserViewModel>();

		foreach (var user in users)
		{
			if (user.UserName != User.Identity.Name)
			{
				var userRole = await _userManager.GetRolesAsync(user);
				UserRoles.Add(new UserViewModel
				{
					User = user,
					Roles = userRole,
				});

			}

		}
		return View(UserRoles);
	}

	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> ActiveDeactive(string id)
	{
		var user = await _userManager.FindByNameAsync(id);

		if (user == null)
		{
			return NotFound();
		}
		return View(user);
	}

	[HttpPost]
	[Authorize(Roles = "Admin")]
	[ActionName("ActiveDeactive")]
	public async Task<IActionResult> ActivateOrDeactivate(string id)
	{
		var user = await _userManager.FindByNameAsync(id);
		if (user == null)
		{
			return NotFound();
		}
		if (user.IsActive)
		{
			user.IsActive = false;
		}
		else
		{
			user.IsActive = true;
		}
		await _context.SaveChangesAsync();

        return Json(new { message = "Success!" });
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ChangeRoles(string id)
    {
        var user = await _userManager.FindByNameAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var roles = await _roleManager.Roles.ToListAsync();
        var userRoles = await _userManager.GetRolesAsync(user);

        var model = new ChangeRolesViewModel
        {
            UserId = user.Id,
            UserName = user.UserName,
            UserRoles = userRoles,
            AllRoles = roles
        };

        return View(model);
    }
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ChangeRoles(ChangeRolesViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null)
        {
            return NotFound();
        }

        var selectedRoles = model.SelectedRoles ?? new string[] { };
        var userRoles = await _userManager.GetRolesAsync(user);

        var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Failed to add selected roles to user.");
            return View(model);
        }

        result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Failed to remove deselected roles from user.");
            return View(model);
        }

        return RedirectToAction("Index");
    }

}
