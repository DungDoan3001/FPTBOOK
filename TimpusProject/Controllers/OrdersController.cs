using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimpusProject.Models;
using TimpusProject.ModelView;


namespace TimpusProject.Controllers
{
    public class OrdersController : Controller
    {
        private readonly TimpusDBContext _context;
        public INotyfService _notyfService { get; }
        public OrdersController(TimpusDBContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        [HttpPost]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                var accountID = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(accountID)) return RedirectToAction("Login", "Accounts");
                var customer = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(accountID));
                if (customer == null) return NotFound();
                var orders = await _context.Orders
                    .Include(x => x.TransacStatus)
                    .FirstOrDefaultAsync(m => m.OrderId == id && Convert.ToInt32(accountID) == m.CustomerId);
                if (orders == null) return NotFound();

                var orderDetails = _context.OrderDetails
                    .Include(x => x.Product)
                    .AsNoTracking()
                    .Where(x => x.OrderId == id)
                    .OrderBy(x => x.OrderDetailId)
                    .ToList();
                ViewOrders Orders = new ViewOrders();
                Orders.Orders = orders;
                Orders.OrderDetails = orderDetails;
                return PartialView("Details", Orders);
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
