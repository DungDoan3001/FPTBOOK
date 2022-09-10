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
    public class OrdersController : Controller
    {
        private readonly TimpusDBContext _context;
        public INotyfService _notifyService { get; }

        public OrdersController(TimpusDBContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/Orders
        public async Task<IActionResult> Index()
        {
            var accountID = HttpContext.Session.GetString("AccountId");
            if (string.IsNullOrEmpty(accountID))
            {
                _notifyService.Warning("You need to login with admin account");
                return RedirectToAction("Login", "LoginAdmin");
            }
            var orders = _context.Orders
                .AsNoTracking()
                .OrderBy(order => order.TransacStatusId)
                .Include(o => o.Customer)
                .Include(o => o.TransacStatus)
                .ToListAsync();

            return View(await orders);
        }

        public async Task<IActionResult> Pending()
        {
            var orders = _context.Orders
                .AsNoTracking()
                .Where(order => order.TransacStatusId == 1)
                .OrderBy(order => order.TransacStatusId)
                .Include(o => o.Customer)
                .Include(o => o.TransacStatus)
                .ToListAsync();

            return View(await orders);
        }

        public async Task<IActionResult> OnGoing()
        {
            var orders = _context.Orders
                .AsNoTracking()
                .Where(order => order.TransacStatusId == 2)
                .OrderBy(order => order.TransacStatusId)
                .Include(o => o.Customer)
                .Include(o => o.TransacStatus)
                .ToListAsync();

            return View(await orders);
        }

        public async Task<IActionResult> Finished()
        {
            var orders = _context.Orders
                .AsNoTracking()
                .Where(order => order.TransacStatusId == 3)
                .OrderBy(order => order.TransacStatusId)
                .Include(o => o.Customer)
                .Include(o => o.TransacStatus)
                .ToListAsync();

            return View(await orders);
        }


        // GET: Admin/Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.TransacStatus)
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(m => m.OrderId == id);

            List<Product> lsproducts = new List<Product>();
            foreach(var orderDetail in order.OrderDetails)
            {
                lsproducts.Add(_context.Products.FirstOrDefault(product => product.ProductId == orderDetail.ProductId));
            }
            if (order == null)
            {
                return NotFound();
            }

            ViewData["Products"] = lsproducts.ToList();

            return View(order);
        }

        public IActionResult Update(int? OrderId)
        {
            var order = _context.Orders
                .Include(order => order.TransacStatus)
                .FirstOrDefault(order => order.OrderId == OrderId);

            if(order == null)
            {
                return NotFound();
            }

            if(order.TransacStatusId < 3 && order.TransacStatusId > 0)
            {
                order.TransacStatusId += 1;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    _context.SaveChanges();
                    _notifyService.Success("Update successfully!");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            return RedirectToAction("Index");
        }

        public IActionResult Decline(int? OrderId)
        {
            var order = _context.Orders
                .Include(order => order.TransacStatus)
                .FirstOrDefault(order => order.OrderId == OrderId);

            if (order == null)
            {
                return NotFound();
            }

            order.TransacStatusId = 4;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    _context.SaveChanges();
                    _notifyService.Success("Update successfully!");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            return RedirectToAction("Index");
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }


    }
}
