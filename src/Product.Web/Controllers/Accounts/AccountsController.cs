using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.Service.Commons.Helpers;
using Product.Service.DTOs.Accounts;
using Product.Service.DTOs.Admins;
using Product.Service.Exceptions;
using Product.Service.Interfaces.Accounts;

namespace Product.Web.Controllers.Accounts
{
    public class AccountsController : Controller
    {
        private readonly IAccountService _service;

        public AccountsController(IAccountService accountService)
        {
            _service = accountService;
        }

        [HttpGet("register")]
        [Authorize(Roles = "admin, superadmin")]
        public ViewResult Register() => View("Register");

        [HttpPost("register")]
        public async Task<IActionResult> AdminRegisterAsync(AdminRegisterDto adminRegisterDto)
        {
            if (ModelState.IsValid)
            {
                bool result = await _service.AdminRegisterAsync(adminRegisterDto);
                if (result)
                {
                    return RedirectToAction("login", "accounts", new { area = "" });
                }
                else
                {
                    return Register();
                }
            }
            else return Register();
        }

        [HttpGet("login")]
        public ViewResult Login() => View("Login");

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(AccountLoginDto accountLoginDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string token = await _service.LoginAsync(accountLoginDto);
                    HttpContext.Response.Cookies.Append("X-Access-Token", token, new CookieOptions()
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict
                    });
                    TempData["SuccessMessage"] = $"You have successfully entered the admin panel";

                    return RedirectToAction("Index", "Products", new { area = "" });
                }
                catch (ModelErrorException modelError)
                {
                    TempData["InfoMessage"] = $"You have entered an incorrect password or login!";
                    TempData["ErrorMessage"] = "An error occurred: " + modelError.Message;
                    ModelState.AddModelError(modelError.Property, modelError.Message);
                    return Login();
                }
                catch(Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred: " + ex.Message;

                    return Login();
                }
            }
            else return Login();
        }

        [HttpGet("logout")]
        public IActionResult LogOut()
        {
            HttpContext.Response.Cookies.Append("X-Access-Token", "", new CookieOptions()
            {
                Expires = TimeHelper.GetCurrentServerTime().AddDays(-1)
            });
            return RedirectToAction("login", "accounts", new { area = "" });
        }
    }
}
