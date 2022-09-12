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
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TimpusDBContext _context;

        public HomeController(ILogger<HomeController> logger, TimpusDBContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var lsHomeFlagProducts = _context.Products
            .AsNoTracking()
            .Where(product => product.Active == true)
            .Where(product => product.Cat.Published == true)
            .Where(product => product.HomeFlag == true)
            .Include(product => product.Cat)
            .OrderByDescending(product => product.DateModified)
            .ToList();

            var lsBestSellers = _context.Products
            .AsNoTracking()
            .Where(product => product.Active == true)
            .Where(product => product.Cat.Published == true)
            .Where(product => product.BestSellers == true)
            .Include(product => product.Cat)
            .OrderByDescending(product => product.DateModified)
            .ToList();

            var lsCategories = _context.Categories
            .Where(cat => cat.Published == true)
            .AsNoTracking()
            .ToList();
            ViewData["Categories"] = lsCategories;

            ViewData["HomeFlag"] = lsHomeFlagProducts;
            ViewData["BestSellers"] = lsBestSellers;
            return View();

        }

        public IActionResult About()
        {
            var lsCategories = _context.Categories
            .AsNoTracking()
            .ToList();

            ViewData["Categories"] = lsCategories;

            return View();
        }

        public IActionResult Contact()
        {
            var lsCategories = _context.Categories
            .AsNoTracking()
            .ToList();

            ViewData["Categories"] = lsCategories;
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
