using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimpusProject.Models;

namespace TimpusProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomersController : Controller
    {
        private readonly TimpusDBContext _context;
        public INotyfService _notifyService { get; }
        public CustomersController(TimpusDBContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/Customers
        public async Task<IActionResult> Index()
        {
            var LoggedaccountID = HttpContext.Session.GetString("AccountId");
            if (string.IsNullOrEmpty(LoggedaccountID))
            {
                _notifyService.Warning("You need to login with admin account");
                return RedirectToAction("Login", "LoginAdmin");
            }
            int Loggedid = Int32.Parse(LoggedaccountID);
            var LoggedAccount = _context.Accounts
                    .AsNoTracking()
                    .Include(account => account.Role)
                    .FirstOrDefault(account => account.AccountId == Loggedid);
            ViewBag.Account = LoggedAccount;

            return View(await _context.Customers.ToListAsync());
        }

        // GET: Admin/Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var LoggedaccountID = HttpContext.Session.GetString("AccountId");
            if (string.IsNullOrEmpty(LoggedaccountID))
            {
                _notifyService.Warning("You need to login with admin account");
                return RedirectToAction("Login", "LoginAdmin");
            }
            int Loggedid = Int32.Parse(LoggedaccountID);
            var LoggedAccount = _context.Accounts
                    .AsNoTracking()
                    .Include(account => account.Role)
                    .FirstOrDefault(account => account.AccountId == Loggedid);
            ViewBag.Account = LoggedAccount;

            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Admin/Customers/Create
        public IActionResult Create()
        {
            var LoggedaccountID = HttpContext.Session.GetString("AccountId");
            if (string.IsNullOrEmpty(LoggedaccountID))
            {
                _notifyService.Warning("You need to login with admin account");
                return RedirectToAction("Login", "LoginAdmin");
            }
            int Loggedid = Int32.Parse(LoggedaccountID);
            var LoggedAccount = _context.Accounts
                    .AsNoTracking()
                    .Include(account => account.Role)
                    .FirstOrDefault(account => account.AccountId == Loggedid);
            ViewBag.Account = LoggedAccount;

            return View();
        }

        // POST: Admin/Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,FullName,Avatar,Address,Email,Phone,CreateDate,Username,Password,LastLogin,Salt,Active")] Customer customer)
        {
            var LoggedaccountID = HttpContext.Session.GetString("AccountId");
            if (string.IsNullOrEmpty(LoggedaccountID))
            {
                _notifyService.Warning("You need to login with admin account");
                return RedirectToAction("Login", "LoginAdmin");
            }
            int Loggedid = Int32.Parse(LoggedaccountID);
            var LoggedAccount = _context.Accounts
                    .AsNoTracking()
                    .Include(account => account.Role)
                    .FirstOrDefault(account => account.AccountId == Loggedid);
            ViewBag.Account = LoggedAccount;

            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                _notifyService.Success("Create successfully!");
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Admin/Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var LoggedaccountID = HttpContext.Session.GetString("AccountId");
            if (string.IsNullOrEmpty(LoggedaccountID))
            {
                _notifyService.Warning("You need to login with admin account");
                return RedirectToAction("Login", "LoginAdmin");
            }
            int Loggedid = Int32.Parse(LoggedaccountID);
            var LoggedAccount = _context.Accounts
                    .AsNoTracking()
                    .Include(account => account.Role)
                    .FirstOrDefault(account => account.AccountId == Loggedid);
            ViewBag.Account = LoggedAccount;

            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Admin/Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,FullName,Avatar,Address,Email,Phone,CreateDate,Username,Password,LastLogin,Salt,Active")] Customer customer)
        {
            var LoggedaccountID = HttpContext.Session.GetString("AccountId");
            if (string.IsNullOrEmpty(LoggedaccountID))
            {
                _notifyService.Warning("You need to login with admin account");
                return RedirectToAction("Login", "LoginAdmin");
            }
            int Loggedid = Int32.Parse(LoggedaccountID);
            var LoggedAccount = _context.Accounts
                    .AsNoTracking()
                    .Include(account => account.Role)
                    .FirstOrDefault(account => account.AccountId == Loggedid);
            ViewBag.Account = LoggedAccount;

            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("Edit successfully!");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Admin/Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var LoggedaccountID = HttpContext.Session.GetString("AccountId");
            if (string.IsNullOrEmpty(LoggedaccountID))
            {
                _notifyService.Warning("You need to login with admin account");
                return RedirectToAction("Login", "LoginAdmin");
            }
            int Loggedid = Int32.Parse(LoggedaccountID);
            var LoggedAccount = _context.Accounts
                    .AsNoTracking()
                    .Include(account => account.Role)
                    .FirstOrDefault(account => account.AccountId == Loggedid);
            ViewBag.Account = LoggedAccount;
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Admin/Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            _notifyService.Success("Delete successfully!");
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
