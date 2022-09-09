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


namespace TimpusProject.Controllers
{
    public class Products : Controller
    {
        private readonly TimpusDBContext _context;

        public Products(TimpusDBContext context)
        {
            _context = context;
        }

        public IActionResult Index(int CatId = 0)
        {
            var lsCategories = _context.Categories
            .AsNoTracking()
            .ToList();

            ViewData["Categories"] = lsCategories;

            List<Product> lsProducts = new List<Product>();

            if(CatId != 0)
            {
                lsProducts = _context.Products
                .AsNoTracking()
                .Where(product => product.CatId == CatId)
                .ToList();
            } else
            {
                lsProducts = _context.Products
                .AsNoTracking()
                .ToList();
            }

            ViewBag.CurrentCatId = CatId;
            return View(lsProducts);
        }

        public IActionResult Detail(int? id)
        {
            var lsCategories = _context.Categories
            .AsNoTracking()
            .ToList();

            var product = _context.Products
            .Include(product => product.Cat)
            .Include(product => product.Publisher)
            .FirstOrDefault(product => product.ProductId == id);

            var authors = _context.AuthorProducts
                .Where(authors => authors.ProductId == id)
                .Include(authors => authors.Author)
                .ToList();

            if (product == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["Categories"] = lsCategories;
            ViewData["Authors"] = authors;

            return View(product);
        }
    }
}
