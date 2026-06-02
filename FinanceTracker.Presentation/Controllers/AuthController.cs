using Microsoft.AspNetCore.Mvc;
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Application.DTOs;

namespace FinanceTracker.Presentation.Controllers;


public class AuthController : Controller
{
    private readonly IAuthenticationService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthenticationService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Register(string? returnUrl)
    {
        ViewData["ReturnUrl"] = returnUrl;

        returnUrl ??= Url.Content("~/");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterUserDto model, string returnUrl = null!)
    {
        if (!ModelState.IsValid)
            return View(model);

        ViewData["ReturnUrl"] = returnUrl;

        var (success, message, token) = await _authService.RegisterAsync(model);

        if (!success)
        {
            ModelState.AddModelError(string.Empty, message);
            return View(model);
        }

        // Store token in session or cookie
        //HttpContext.Session.SetString("AuthToken", token ?? string.Empty);
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl)
    {
        ViewData["ReturnUrl"] = returnUrl;
        returnUrl ??= Url.Content("~/");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginUserDto model, string returnUrl = null!)
    {
        if (!ModelState.IsValid)
            return View(model);

        ViewData["ReturnUrl"] = returnUrl;
        var (success, message, token) = await _authService.LoginAsync(model);

        if (!success)
        {
            ModelState.AddModelError(string.Empty, message);
            return View(model);
        }

        //HttpContext.Session.SetString("AuthToken", token ?? string.Empty);
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet]
    public async Task<IActionResult> Logout(string returnUrl)
    {
        await _authService.LogoutAsync();
        return Redirect(returnUrl);
    }
}