using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TimpusProject.Models;

namespace TimpusProject.Controllers
{
    public class Products : Controller
    {
        private readonly TimpusDBContext _context;

        public Products(TimpusDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var lsCategories = _context.Categories
            .AsNoTracking()
            .ToList();

            ViewData["Categories"] = lsCategories;

            return View();
        }

        public IActionResult Detail(int? id)
        {
            var lsCategories = _context.Categories
            .AsNoTracking()
            .ToList();

            var product = _context.Products
            .Include(product => product.Cat)
            .FirstOrDefault(product => product.ProductId == id);

            if (product == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["Categories"] = lsCategories;

            return View(product);
        }
    }
}
