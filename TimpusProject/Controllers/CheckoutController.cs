using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimpusProject.Extension;
using TimpusProject.Helper;
using TimpusProject.Models;
using TimpusProject.ModelView;

namespace TimpusProject.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly TimpusDBContext _context;
        public INotyfService _notyfService { get; }
        public CheckoutController(TimpusDBContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        public List<CartItem> Cart
        {
            get
            {
                var cart = HttpContext.Session.Get<List<CartItem>>("Cart");
                if (cart == default(List<CartItem>))
                {
                    cart = new List<CartItem>();
                }
                return cart;
            }
        }
        [Route("checkout.html", Name = "Checkout")]
        public IActionResult Index(string returnUrl = null)
        {
            var accountID = HttpContext.Session.GetString("CustomerId");
            if (string.IsNullOrEmpty(accountID))
            {
                _notyfService.Warning("You need to login to order products");
                return RedirectToAction("Login", "Accounts");
            }
            
            //Lay gio hang ra de xu ly
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart");
            BuyVM model = new BuyVM();

            var lsCategories = _context.Categories
            .AsNoTracking()
            .ToList();
            ViewData["Categories"] = lsCategories;

            if (accountID != null)
            {
                var customer = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(accountID));
                model.CustomerId = customer.CustomerId;
                model.FullName = customer.FullName;
                model.Email = customer.Email;
                model.Phone = customer.Phone;
                model.Address = customer.Address;
            }
            ViewBag.Cart = cart;
            return View(model);
        }

        [HttpPost]
        [Route("checkout.html", Name = "Checkout")]
        public IActionResult Index(BuyVM buy)
        {
            //Lay ra gio hang de xu ly
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart");
            var accountID = HttpContext.Session.GetString("CustomerId");
            BuyVM model = new BuyVM();
            if (accountID != null)
            {
                var customer = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(accountID));
                model.CustomerId = customer.CustomerId;
                model.FullName = customer.FullName;
                model.Email = customer.Email;
                model.Phone = customer.Phone;
                model.Address = customer.Address;
                customer.Address = buy.Address;
                _context.Update(customer);
                _context.SaveChanges();
            }
            try
            {
                if (ModelState.IsValid)
                    {
                    //Khoi tao don hang
                    Order order = new Order();
                    order.CustomerId = model.CustomerId;
                    order.Address = model.Address;
                    order.OrderDate = DateTime.Now;
                    order.TransacStatusId = 1;//Don hang moi
                    order.Deleted = false;
                    order.Paid = false;
                    order.Total = Convert.ToInt32(cart.Sum(x => x.TotalMoney));
                    _context.Add(order);
                    _context.SaveChanges();
                    //tao danh sach don hang

                    foreach (var item in cart)
                    {
                        OrderDetail orderDetail = new OrderDetail();
                        orderDetail.OrderId = order.OrderId;
                        orderDetail.ProductId = item.product.ProductId;
                        orderDetail.Quantity = item.amount;
                        orderDetail.Total = order.Total;
                        orderDetail.Price = item.product.Price;
                        orderDetail.CreateDate = DateTime.Now;
                        _context.Add(orderDetail);
                    }
                    _context.SaveChanges();
                    //clear gio hang
                    HttpContext.Session.Remove("Cart");
                    //Xuat thong bao
                    _notyfService.Success("Order Successfully");
                    //cap nhat thong tin khach hang
                    //Return về my-profile và hiển thị thông tin đơn hàng
                    //return RedirectToAction("Success");
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                ViewBag.Cart = cart;
                return View(model);
            }
            ViewBag.Cart = cart;
            return View(model);
        }

        [Route("dat-hang-thanh-cong.html", Name = "Success")]
        public IActionResult Success()
        {
            try
            {
                var accountID = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(accountID))
                {
                    return RedirectToAction("Login", "Accounts", new { returnUrl = "/dat-hang-thanh-cong.html" });
                }
                var customer = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(accountID));
                var order = _context.Orders
                    .Where(x => x.CustomerId == Convert.ToInt32(accountID))
                    .OrderByDescending(x => x.OrderDate)
                    .FirstOrDefault();
                BuySuccessVM successVM = new BuySuccessVM();
                successVM.OrderID = order.OrderId;
                successVM.FullName = customer.FullName;
                successVM.Phone = customer.Phone;
                successVM.Address = customer.Address;
                return View(successVM);
            }
            catch
            {
                return View();
            }
        }
    }
}
