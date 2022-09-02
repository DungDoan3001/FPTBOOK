using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimpusProject.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using PagedList.Core;

namespace TimpusProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountsController : Controller
    {
        private readonly TimpusDBContext _context;
        public INotyfService _notifyService { get; }

        public AccountsController(TimpusDBContext context, INotyfService _notifyService)
        {
            _context = context;
        }

        //// GET: Admin/Accounts
        //public async Task<IActionResult> Index()
        //{
        //    ViewData["Roles"] = new SelectList(_context.Roles, "RoleId", "RoleName");

        //    var timpusDBContext = _context.Accounts.Include(a => a.Role);
        //    return View(await timpusDBContext.ToListAsync());
        //}

        // GET: Admin/Accounts
        public IActionResult Index(int page = 1, int RoleId = 0)
        {
            var pageNumber = page;
            var pageSize = 20;

            List<Account> lsAccounts = new List<Account>();

            if(RoleId != 0)
            {
                lsAccounts = _context.Accounts
                    .AsNoTracking()
                    .Where(account => account.RoleId == RoleId)
                    .Include(account => account.Role)
                    .OrderByDescending(account => account.AccountId)
                    .ToList();
            } else
            {
                lsAccounts = _context.Accounts
                    .AsNoTracking()
                    .Include(account => account.Role)
                    .OrderByDescending(account => account.AccountId)
                    .ToList();
            }

            PagedList<Account> models = new PagedList<Account>(lsAccounts.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentRoleId = RoleId;
            ViewBag.CurrentPage = pageNumber;
            ViewData["Roles"] = new SelectList(_context.Roles, "RoleId", "RoleName", RoleId);

            return View(models);
        }


        public IActionResult Filtter(int? page, int RoleId = 0)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var url = $"/Admin/Accounts?RoleId={RoleId}";
            if(RoleId == 0)
            {
                url = $"/Admin/Accounts";
            }

            return Json(new { status = "success", redirectUrl = url });
        }


        // GET: Admin/Accounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Admin/Accounts/Create
        public IActionResult Create()
        {
            ViewData["Roles"] = new SelectList(_context.Roles, "RoleId", "RoleName", "2");
            return View();
        }

        // POST: Admin/Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountId,FullName,Email,Phone,Username,Password,Active,Avatar,RoleId,LastLogin,CreateDate")] Account account)
        {
            if (ModelState.IsValid)
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", account.RoleId);
            return View(account);
        }

        // GET: Admin/Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", account.RoleId);
            return View(account);
        }

        // POST: Admin/Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId,FullName,Email,Phone,Username,Password,Active,Avatar,RoleId,LastLogin,CreateDate")] Account account)
        {
            if (id != account.AccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountId))
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
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", account.RoleId);
            return View(account);
        }

        // GET: Admin/Accounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Admin/Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            //_notifyService.Success("Delete Successfully!");
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountId == id);
        }
    }
}
