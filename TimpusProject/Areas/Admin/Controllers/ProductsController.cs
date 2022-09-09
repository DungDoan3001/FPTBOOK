using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
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
        public INotyfService _notifyService { get; }
        public ProductsController(TimpusDBContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
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
                    .OrderByDescending(product => product.HomeFlag)
                    .ThenByDescending(product => product.BestSellers)
                    .ToList();
                }
                else
                {
                    lsProducts = _context.Products
                    .AsNoTracking()
                    .Where(product => product.CatId == CatId)
                    .Where(product => product.Active == false)
                    .Include(product => product.Cat)
                    .OrderByDescending(product => product.HomeFlag)
                    .ThenByDescending(product => product.BestSellers)
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
                    .OrderByDescending(product => product.HomeFlag)
                    .ThenByDescending(product => product.BestSellers)
                    .ToList();
                }
                else
                {
                    lsProducts = _context.Products
                    .AsNoTracking()
                    .Where(product => product.Active == false)
                    .Include(product => product.Cat)
                    .OrderByDescending(product => product.HomeFlag)
                    .ThenByDescending(product => product.BestSellers)
                    .ToList();
                }
            }
            else if (Active == 0 && CatId > 0)
            {
                lsProducts = _context.Products
                    .AsNoTracking()
                    .Where(product => product.CatId == CatId)
                    .Include(product => product.Cat)
                    .OrderByDescending(product => product.HomeFlag)
                    .ThenByDescending(product => product.BestSellers)
                    .ToList();
            }
            else
            {
                lsProducts = _context.Products
                    .AsNoTracking()
                    .Include(product => product.Cat)
                    .OrderByDescending(product => product.HomeFlag)
                    .ThenByDescending(product => product.BestSellers)
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
                .Include(p => p.AuthorProducts)
                .FirstOrDefaultAsync(m => m.ProductId == id);


            if (product == null)
            {
                return NotFound();
            }

            var selected_authors = _context.AuthorProducts
                .AsNoTracking()
                .Where(author => author.ProductId == id)
                .Include(author => author.Author)
                .ToList();

            var authors = _context.Authors
                           .AsNoTracking()
                           .ToList();

            ViewData["SelectedAuthor"] = selected_authors;
            ViewData["Authors"] = authors;
            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "FullName", product.PublisherId);
            return View(product);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            var authors = _context.Authors
                .AsNoTracking()
                .ToList();

            ViewData["Authors"] = authors;
            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatName");
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "FullName");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,Description,CatId,Price,Thumb,DateCreated,DateModified,BestSellers,HomeFlag,Active,UnitInStock,SmallDescription,PublisherId,Isbn")] Product product, List<int> authors)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                var addedProduct = _context.Products
                    .OrderByDescending(product => product.ProductId)
                    .First();
                
                if (authors != null)
                {
                    foreach (var author in authors)
                    {
                        var author_product = new AuthorProduct { ProductId = addedProduct.ProductId, AuthorId = author };
                        _context.AuthorProducts.Add(author_product);
                    }
                }

                await _context.SaveChangesAsync();
                _notifyService.Success("Create successfully!");
                return RedirectToAction(nameof(Index));
            }
            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatId", product.CatId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "FullName", product.PublisherId);
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

            var selected_authors = _context.AuthorProducts
                .AsNoTracking()
                .Where(author => author.ProductId == id)
                .Include(author => author.Author)
                .ToList();

            var authors = _context.Authors
                           .AsNoTracking()
                           .ToList();

            ViewData["SelectedAuthor"] = selected_authors;
            ViewData["Authors"] = authors;
            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "FullName", product.PublisherId);

            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,Description,CatId,Price,Thumb,DateCreated,DateModified,BestSellers,HomeFlag,Active,UnitInStock,SmallDescription,PublisherId,Isbn")] Product product, List<int> authors)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (authors != null)
                    {
                        var author_product_ls = _context.AuthorProducts.Where(author => author.ProductId == id).ToList();
                        foreach(var item in author_product_ls)
                        {
                            _context.AuthorProducts.Remove(item);
                        }
                        foreach (var author in authors)
                        {
                            var author_product_edit = new AuthorProduct { ProductId = product.ProductId, AuthorId = author };
                            _context.AuthorProducts.Add(author_product_edit);
                        }
                    }
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("Edit successfully!");
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
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "FullName", product.PublisherId);
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

            var selected_authors = _context.AuthorProducts
                .AsNoTracking()
                .Where(author => author.ProductId == id)
                .Include(author => author.Author)
                .ToList();

            var authors = _context.Authors
                           .AsNoTracking()
                           .ToList();

            ViewData["SelectedAuthor"] = selected_authors;
            ViewData["Authors"] = authors;
            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "FullName", product.PublisherId);
            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            var author_product = _context.AuthorProducts
                .AsNoTracking()
                .Where(author_product => author_product.ProductId == id)
                .ToList();

            foreach(var item in author_product)
            {
                _context.AuthorProducts.Remove(item);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            _notifyService.Success("Delete successfully!");
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
