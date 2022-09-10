using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimpusProject.Models;

namespace TimpusProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class LoginAdminController : Controller
    {
        private readonly TimpusDBContext _context;
        public INotyfService _notyfService { get; }
        public LoginAdminController(TimpusDBContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        [AllowAnonymous]
        [Route("Login-admin.html", Name = "Login Admin")]
        public IActionResult Login(string returnUrl = null)
        {
            var accountID = HttpContext.Session.GetString("AccountId");
            if (accountID != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Login-admin.html", Name = "Login Admin")]
        public async Task<IActionResult> Login(Account adminAccount, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var admin = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.Username.Trim() == adminAccount.Username);

                    if (admin == null)
                    {
                        _notyfService.Warning("Login information is incorrect");
                        return RedirectToAction("Login", "LoginAdmin");
                    }

                    if (admin.Active == false)
                    {
                        _notyfService.Warning("This account is disabled");
                        return RedirectToAction("Login", "LoginAdmin");
                    }

                    string pass = adminAccount.Password;
                    if (admin.Password != pass)
                    {
                        _notyfService.Warning("Login information is incorrect");
                        return View(adminAccount);
                    }

                    //Luu Session MaKh
                    HttpContext.Session.SetString("AccountId", admin.AccountId.ToString());
                    var accountID = HttpContext.Session.GetString("AccountId");

                    //Identity
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, admin.FullName),
                        new Claim("AccountId", admin.AccountId.ToString())
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    _notyfService.Success("Login successful!");
                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return Redirect(returnUrl);
                    }
                }
            }
            catch
            {
                _notyfService.Error("Error found!");
                return RedirectToAction("Login", "LoginAdmin");
            }
            return View(adminAccount);
        }

        [HttpGet]
        [Route("admin-log-out.html", Name = "Logout Admin")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("AccountId");
            _notyfService.Success("Logout successful!");
            return RedirectToAction("Index", "Home");
        }
    }
}
