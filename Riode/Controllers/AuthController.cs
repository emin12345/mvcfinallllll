using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Riode.Helpers;
using Riode.Models;
using Riode.ViewModels;

namespace Riode.Controllers;

public class AuthController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Login()
    {
        if (User.Identity.IsAuthenticated)
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        if (User.Identity.IsAuthenticated)
            return RedirectToAction("Index", "Home");

        if (!ModelState.IsValid)
            return View();

        var user = await _userManager.FindByNameAsync(loginViewModel.UsernameOrEmail);
        if (user == null)
        {
            user = await _userManager.FindByEmailAsync(loginViewModel.UsernameOrEmail);
            if (user == null)
            {
                ModelState.AddModelError("", "Username/email or password incorrect");
                return View();
            }
        }

        if (!await _userManager.IsEmailConfirmedAsync(user))
        {
            ModelState.AddModelError("", "Please confirm your email address");
            return View();
        }

        if (!user.IsActive)
        {
			ModelState.AddModelError("", "Your account is not active");
			return View();
		}

        var signInResult = await _signInManager.PasswordSignInAsync(user.UserName, loginViewModel.Password, loginViewModel.RememberMe, true);
        if (signInResult.IsLockedOut)
        {
            ModelState.AddModelError("", "Get, sonra gelersen...");
            return View();
        }
        if (!signInResult.Succeeded)
        {
            ModelState.AddModelError("", "Username/email or password incorrect");
            return View();
        }

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        if (!User.Identity.IsAuthenticated)
            return BadRequest();

        await _signInManager.SignOutAsync();

        return RedirectToAction("Index", "Home");
    }

    public IActionResult ForgotPassword()
    {
        return View("LoginRegister");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPasswordViewModel)
    {
        if (!ModelState.IsValid)
            return View();

        var user = await _userManager.FindByEmailAsync(forgotPasswordViewModel.Email);
        if (user == null)
        {
            ModelState.AddModelError("Email", "Email is not found");
            return View();
        }

        // https://localhost:7145/Auth/ResetPassword?email=&token=
        string token = await _userManager.GeneratePasswordResetTokenAsync(user);
        string link = Url.Action("ResetPassword", "Auth", new { email = user.Email, token }, HttpContext.Request.Scheme, HttpContext.Request.Host.Value);

        string path = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "templates", "index.html");
        using StreamReader streamReader = new StreamReader(path);
        string content = await streamReader.ReadToEndAsync();
        string body = content.Replace("[link]", link);

        EmailHelper emailHelper = new EmailHelper(_configuration);
        await emailHelper.SendEmailAsync(new MailRequest { ToEmail = user.Email, Subject = "ResetPassword", Body = body });

        return RedirectToAction(nameof(Login));
    }

    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
    {
        var user = await _userManager.FindByEmailAsync(resetPasswordViewModel.Email);
        if (user == null)
            return NotFound();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(SubmitResetPasswordViewModel submitResetPasswordViewModel, string email, string token)
    {
        if (!ModelState.IsValid)
            return View();

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return NotFound();

        IdentityResult identityResult = await _userManager.ResetPasswordAsync(user, token, submitResetPasswordViewModel.Password);
        if (!identityResult.Succeeded)
        {
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View();
        }

        return RedirectToAction(nameof(Login));
    }

    public async Task<IActionResult> ConfirmEmail(ConfirmEmailViewModel confirmEmailViewModel)
    {
        var user = await _userManager.FindByEmailAsync(confirmEmailViewModel.Email);
        if (user == null)
            return NotFound();

        if (await _userManager.IsEmailConfirmedAsync(user))
            return BadRequest();

        IdentityResult identityResult = await _userManager.ConfirmEmailAsync(user, confirmEmailViewModel.Token);
        if (identityResult.Succeeded)
        {
            TempData["ConfirmationMessage"] = "Your email is successfully confirmed";
            return RedirectToAction(nameof(Login));
        }

        return BadRequest(); 
    }
}
