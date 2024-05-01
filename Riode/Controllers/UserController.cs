using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Riode.Helpers;
using Riode.Helpers.Enums;
using Riode.Models;
using Riode.ViewModels;
using System.Data;

namespace Riode.Controllers;

public class UserController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;
	public SignInManager<AppUser> _signInManager { get; }

	public UserController(UserManager<AppUser> userManager, IConfiguration configuration, SignInManager<AppUser> signInManager)
	{
		_userManager = userManager;
		_configuration = configuration;
		_signInManager = signInManager;
	}

	public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        AppUser appUser = new AppUser()
        {
            FullName = registerViewModel.FullName,
            UserName = registerViewModel.Username,
            Email = registerViewModel.Email,
            IsActive = true
        };

        IdentityResult identityResult = await _userManager.CreateAsync(appUser, registerViewModel.Password);
        if (!identityResult.Succeeded)
        {
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View();
        }

        string token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

        string link = Url.Action("ConfirmEmail", "Auth", new { email = appUser.Email, token }, HttpContext.Request.Scheme, HttpContext.Request.Host.Value);

        string body = $"<a href='{link}'>Confirm your email</a>";

        EmailHelper emailHelper = new EmailHelper(_configuration);
        await emailHelper.SendEmailAsync(new MailRequest { ToEmail = appUser.Email, Subject = "Confirm Email", Body = body });

        await _userManager.AddToRoleAsync(appUser, Roles.User.ToString());

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        if (user == null)
            return NotFound();

        UserUpdateViewModel userUpdateViewModel = new()
        {
            FullName = user.FullName,
            UserName = user.UserName,
            Email = user.Email
        };


        return View(userUpdateViewModel);
    }

	[Authorize]
	[HttpPost]
	public async Task<IActionResult> UpdateProfile(UserUpdateViewModel userUpdateProfileViewModel)
	{

		UserProfileViewModel userProfileViewModel = new()
		{
			UserUpdateViewModel = userUpdateProfileViewModel
		};

		if (!ModelState.IsValid)
			return View(nameof(Profile), userProfileViewModel);

		var user = await _userManager.FindByNameAsync(User.Identity.Name);
		if (user == null)
			return NotFound();


		if (user.UserName != userUpdateProfileViewModel.UserName && _userManager.Users.Any(u => u.UserName == userUpdateProfileViewModel.UserName))
		{
			ModelState.AddModelError("UserName", "This Username has already been taken");
			return View(nameof(Profile), userProfileViewModel);
		}

		if (user.Email != userUpdateProfileViewModel.Email && _userManager.Users.Any(u => u.Email == userUpdateProfileViewModel.Email))
		{
			ModelState.AddModelError("Email", "This email has already been taken");
			return View(nameof(Profile), userProfileViewModel);
		}

		if (userUpdateProfileViewModel.CurrentPassword != null)
		{
			if (userUpdateProfileViewModel.NewPassword == null)
			{
				ModelState.AddModelError("NewPassword", "New password is required");
				return View(nameof(Profile), userProfileViewModel);
			}

			IdentityResult identityResult = await _userManager.ChangePasswordAsync(user, userUpdateProfileViewModel.CurrentPassword, userUpdateProfileViewModel.NewPassword);
			if (!identityResult.Succeeded)
			{
				foreach (var error in identityResult.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
				return View(nameof(Profile), userProfileViewModel);
			}
		}

		user.FullName = userUpdateProfileViewModel.FullName;
		user.UserName = userUpdateProfileViewModel.UserName;
		user.Email = userUpdateProfileViewModel.Email;

		IdentityResult result = await _userManager.UpdateAsync(user);
		if (!result.Succeeded)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error.Description);
			}
			return View(nameof(Profile), userProfileViewModel);
		}

		await _signInManager.RefreshSignInAsync(user);

		TempData["SuccessMessage"] = "Your profile updated successfully";

		return RedirectToAction(nameof(Profile));
	}
}
