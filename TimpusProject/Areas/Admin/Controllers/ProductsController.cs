using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using TimpusProject.Models;

namespace TimpusProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly TimpusDBContext _context;

        public ProductsController(TimpusDBContext context)
        {
            _context = context;
        }

        // GET: Admin/Products
        public IActionResult Index(int CatId = 0, int Active = 0)
        {
            List<Product> lsProducts = new List<Product>();

            if (Active > 0 && CatId > 0)
            {
                if (Active == 1)
                {
                    lsProducts = _context.Products
                    .AsNoTracking()
                    .Where(product => product.CatId == CatId)
                    .Where(product => product.Active == true)
                    .Include(product => product.Cat)
                    .OrderByDescending(product => product.ProductId)
                    .ToList();
                }
                else
                {
                    lsProducts = _context.Products
                    .AsNoTracking()
                    .Where(product => product.CatId == CatId)
                    .Where(product => product.Active == false)
                    .Include(product => product.Cat)
                    .OrderByDescending(product => product.ProductId)
                    .ToList();
                };
            }
            else if (Active > 0 && CatId == 0)
            {
                if (Active == 1)
                {
                    lsProducts = _context.Products
                    .AsNoTracking()
                    .Where(product => product.Active == true)
                    .Include(product => product.Cat)
                    .OrderByDescending(product => product.ProductId)
                    .ToList();
                }
                else
                {
                    lsProducts = _context.Products
                    .AsNoTracking()
                    .Where(product => product.Active == false)
                    .Include(product => product.Cat)
                    .OrderByDescending(product => product.ProductId)
                    .ToList();
                }
            }
            else if (Active == 0 && CatId > 0)
            {
                lsProducts = _context.Products
                    .AsNoTracking()
                    .Where(product => product.CatId == CatId)
                    .Include(product => product.Cat)
                    .OrderByDescending(product => product.ProductId)
                    .ToList();
            }
            else
            {
                lsProducts = _context.Products
                    .AsNoTracking()
                    .Include(product => product.Cat)
                    .OrderByDescending(product => product.ProductId)
                    .ToList();
            }

            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatName", CatId);

            ViewData["Active"] = new SelectList(new List<SelectListItem>
            {
                new SelectListItem{Text="Active", Value = "1"},
                new SelectListItem{Text="Disabled", Value = "2"}
            }, "Value", "Text", Active);

            return View(lsProducts.ToList());
        }


        public IActionResult Filtter(int CatId = 0, int Active = 0)
        {
            var url = $"/Admin/Products?CatId={CatId}&Active={Active}";
            if (CatId == 0 && Active == 0)
            {
                url = $"/Admin/Products";
            }
            else if (CatId > 0 && Active == 0)
            {
                url = $"/Admin/Products?CatId={CatId}";
            }
            else if (CatId == 0 && Active > 0)
            {
                url = $"/Admin/Products?Active={Active}";
            }

            return Json(new { status = "success", redirectUrl = url });
        }




        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
            return View(product);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatName");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,Description,CatId,Price,Thumb,DateCreated,DateModified,BestSellers,HomeFlag,Active,UnitInStock")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatId", product.CatId);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,Description,CatId,Price,Thumb,DateCreated,DateModified,BestSellers,HomeFlag,Active,UnitInStock")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatId", product.CatId);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
