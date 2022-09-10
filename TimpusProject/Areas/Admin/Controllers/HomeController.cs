using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimpusProject.Models;

namespace TimpusProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly TimpusDBContext _context;
        public INotyfService _notyfService { get; }
        public HomeController(TimpusDBContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        public IActionResult Index()
        {
            var accountID = HttpContext.Session.GetString("AccountId");
            if (string.IsNullOrEmpty(accountID))
            {
                _notyfService.Warning("You need to login with admin account");
                return RedirectToAction("Login", "LoginAdmin");
            }
            int id = Int32.Parse(accountID);
            List<Account> lsAccount = new List<Account>();
            lsAccount = _context.Accounts
                    .AsNoTracking()
                    .Where(account => account.AccountId == id)
                    .ToList();
            ViewBag.Account = lsAccount;
            return View();
        }
    }
}
