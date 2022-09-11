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
using TimpusProject.Extension;
using TimpusProject.Helper;
using TimpusProject.Models;
using TimpusProject.ModelView;

namespace TimpusProject.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly TimpusDBContext _context;
        public INotyfService _notyfService { get; }
        public AccountsController(TimpusDBContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        [Route("my-profile.html", Name = "Dashboard")]
        public IActionResult Dashboard()
        {
            var lsCategories = _context.Categories
            .AsNoTracking()
            .ToList();
            ViewData["Categories"] = lsCategories;

            var accountID = HttpContext.Session.GetString("CustomerId");
            if (accountID != null)
            {
                var customer = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(accountID));
                if (customer != null)
                {
                    var lsOrder = _context.Orders
                        .Include(x => x.TransacStatus)
                        .AsNoTracking()
                        .Where(x => x.CustomerId == customer.CustomerId)
                        .OrderByDescending(x => x.OrderDate)
                        .ToList();
                    ViewBag.Order = lsOrder;
                    return View(customer);
                }

            }
            return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidatePhone(string Phone)
        {
            try
            {
                var customers = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Phone.ToLower() == Phone.ToLower());
                if (customers != null)
                    return Json(data: "Phone number: " + Phone + "has used");

                return Json(data: true);

            }
            catch
            {
                return Json(data: true);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidateEmail(string Email)
        {
            try
            {
                var customers = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email.ToLower() == Email.ToLower());
                if (customers != null)
                    return Json(data: "Email : " + Email + " has used");
                return Json(data: true);
            }
            catch
            {
                return Json(data: true);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("RegisterAccount.html", Name = "Register account")]
        public IActionResult RegisterAccount()
        {
            var lsCategories = _context.Categories
            .AsNoTracking()
            .ToList();
            ViewData["Categories"] = lsCategories;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("RegisterAccount.html", Name = "Register account")]
        public async Task<IActionResult> RegisterAccount(RegisterViewModel account)
        {
            var lsCategories = _context.Categories
            .AsNoTracking()
            .ToList();
            ViewData["Categories"] = lsCategories;

            try
            {
                if (ModelState.IsValid)
                {
                    var checkMail = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email.Trim().ToLower() == account.Email.Trim().ToLower());
                    var checkUsername = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Username.Trim().ToLower() == account.Username.Trim().ToLower());
                    if (checkMail != null)
                    {
                        _notyfService.Warning("Email already used");
                        return RedirectToAction("RegisterAccount", "Accounts");
                    }
                    if (checkUsername != null)
                    {
                        _notyfService.Warning("Username already used");
                        return RedirectToAction("RegisterAccount", "Accounts");
                    }

                    string salt = Utilities.GetRandomKey();
                    Customer customers = new Customer
                    {
                        FullName = account.FullName,
                        Username = account.Username,
                        Phone = account.Phone.Trim().ToLower(),
                        Email = account.Email.Trim().ToLower(),
                        Password = (account.Password + salt.Trim()).ToMD5(),
                        Active = true,
                        Salt = salt,
                        CreateDate = DateTime.Now
                    };
                    try
                    {
                        _context.Add(customers);
                        await _context.SaveChangesAsync();
                        //Lưu Session MaKh
                        HttpContext.Session.SetString("CustomerId", customers.CustomerId.ToString());
                        var accountID = HttpContext.Session.GetString("CustomerId");

                        //Identity
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name,customers.FullName),
                            new Claim("CustomerId", customers.CustomerId.ToString())
                        };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimsPrincipal);
                        _notyfService.Success("Register Successful!");
                        return RedirectToAction("Dashboard", "Accounts");
                    }
                    catch
                    {
                        return RedirectToAction("RegisterAccount", "Accounts");
                    }
                }
                else
                {
                    return View(account);
                }
            }
            catch
            {
                _notyfService.Error("Error Found!");
                return View(account);
            }
        }

        [AllowAnonymous]
        [Route("Login.html", Name = "Login")]
        public IActionResult Login(string returnUrl = null)
        {
            var lsCategories = _context.Categories
            .AsNoTracking()
            .ToList();
            ViewData["Categories"] = lsCategories;

            var accountID = HttpContext.Session.GetString("CustomerId");
            if (accountID != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Login.html", Name = "Login")]
        public async Task<IActionResult> Login(LoginViewModel customer, string returnUrl)
        {
            var lsCategories = _context.Categories
            .AsNoTracking()
            .ToList();
            ViewData["Categories"] = lsCategories;

            try
            {
                if (ModelState.IsValid)
                {
                    bool isEmail = Utilities.IsValidEmail(customer.Email);
                    if (!isEmail) return View(customer);

                    var customers = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email.Trim() == customer.Email);

                    if (customers == null)
                    {
                        _notyfService.Warning("Login information is incorrect");
                        return RedirectToAction("Login");
                    }

                    if(customers.Active == false)
                    {
                        _notyfService.Warning("This account is disabled");
                        return RedirectToAction("Login");
                    }

                    string pass = (customer.Password + customers.Salt.Trim()).ToMD5();
                    if (customers.Password != pass)
                    {
                        _notyfService.Warning("Login information is incorrect");
                        return View(customer);
                    }

                    //Luu Session MaKh
                    HttpContext.Session.SetString("CustomerId", customers.CustomerId.ToString());
                    var accountID = HttpContext.Session.GetString("CustomerId");

                    //Identity
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, customers.FullName),
                        new Claim("CustomerId", customers.CustomerId.ToString())
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    _notyfService.Success("Login successful!");
                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToAction("Dashboard", "Accounts");
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
                return RedirectToAction("Login", "Accounts");
            }
            return View(customer);
        }

        [HttpGet]
        [Route("Log-out.html", Name = "Logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("CustomerId");
            _notyfService.Success("Logout successful!");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordVM model)
        {
            try
            {
                var accountID = HttpContext.Session.GetString("CustomerId");
                if (accountID == null)
                {
                    return RedirectToAction("Login", "Accounts");
                }
                if (model.Password != model.ConfirmPassword)
                {
                    _notyfService.Warning("Confirm password is wrong!");
                    return RedirectToAction("Dashboard", "Accounts");
                }
                if (ModelState.IsValid)
                {
                    var account = _context.Customers.Find(Convert.ToInt32(accountID));
                    if (account == null) return RedirectToAction("Login", "Accounts");
                    var pass = (model.PasswordNow.Trim() + account.Salt.Trim()).ToMD5();
                    if(pass == account.Password)
                    {
                        string passnew = (model.Password.Trim() + account.Salt.Trim()).ToMD5();
                        account.Password = passnew;
                        _context.Update(account);
                        _context.SaveChanges();
                        _notyfService.Success("Change password successful!");
                        return RedirectToAction("Dashboard", "Accounts");
                    }
                    else
                    {
                        _notyfService.Warning("Current password is wrong!");
                        return RedirectToAction("Dashboard", "Accounts");
                    }
                }
            }
            catch
            {
                _notyfService.Warning("Error found! Cannot change password!");
                return RedirectToAction("Dashboard", "Accounts");
            }
            _notyfService.Warning("Cannot change password!");
            return RedirectToAction("Dashboard", "Accounts");
        }
    }
}
