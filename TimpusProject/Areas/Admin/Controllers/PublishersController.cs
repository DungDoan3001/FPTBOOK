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
    public class PublishersController : Controller
    {
        private readonly TimpusDBContext _context;
        public INotyfService _notifyService { get; }

        public PublishersController(TimpusDBContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/Publishers
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
            return View(await _context.Publishers.ToListAsync());
        }

        // GET: Admin/Publishers/Details/5
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

            var publisher = await _context.Publishers
                .FirstOrDefaultAsync(m => m.PublisherId == id);
            if (publisher == null)
            {
                return NotFound();
            }

            return View(publisher);
        }

        // GET: Admin/Publishers/Create
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

        // POST: Admin/Publishers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PublisherId,Avatar,FullName,Description")] Publisher publisher)
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
                _context.Add(publisher);
                await _context.SaveChangesAsync();
                _notifyService.Success("Create successfully!");
                return RedirectToAction(nameof(Index));
            }
            return View(publisher);
        }

        // GET: Admin/Publishers/Edit/5
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

            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null)
            {
                return NotFound();
            }
            return View(publisher);
        }

        // POST: Admin/Publishers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PublisherId,Avatar,FullName,Description")] Publisher publisher)
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

            if (id != publisher.PublisherId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(publisher);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("Edit successfully!");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PublisherExists(publisher.PublisherId))
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
            return View(publisher);
        }

        // GET: Admin/Publishers/Delete/5
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

            var publisher = await _context.Publishers
                .FirstOrDefaultAsync(m => m.PublisherId == id);
            if (publisher == null)
            {
                return NotFound();
            }

            return View(publisher);
        }

        // POST: Admin/Publishers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
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

            var publisher = await _context.Publishers.FindAsync(id);
            _context.Publishers.Remove(publisher);
            await _context.SaveChangesAsync();
            _notifyService.Success("Delete successfully!");
            return RedirectToAction(nameof(Index));
        }

        private bool PublisherExists(int id)
        {
            return _context.Publishers.Any(e => e.PublisherId == id);
        }
    }
}
