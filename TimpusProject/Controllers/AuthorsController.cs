using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimpusProject.Models;

namespace TimpusProject.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly TimpusDBContext _context;

        public AuthorsController(TimpusDBContext context)
        {
            _context = context;
        }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var author = await _context.Authors
                .FirstOrDefaultAsync(m => m.AuthorId == id);
            if (author == null)
            {
                return NotFound();
            }

            var lsCategories = _context.Categories
            .AsNoTracking()
            .ToList();
            ViewData["Categories"] = lsCategories;

            return View(author);
        }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.AuthorId == id);
        }
    }
}
