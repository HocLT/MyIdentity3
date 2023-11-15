using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyIdentity.Data;

namespace MyIdentity.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Category
        public async Task<IActionResult> Index()
        {
            var data = await _context.Category!
                .Include(c => c.Parent)
                .Include(c => c.Children)
                .ToListAsync();
            var cates = data
                .Where(c => c.Parent == null)
                .ToList();
            return View(cates);
        }

        // GET: Admin/Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        async Task<IEnumerable<Category>> GetItemsSelectCategory()
        {
            var list = await _context.Category!
                .Include(c => c.Children)
                .ToListAsync();

            var listitem = list.Where(c => c.Parent == null)
                .ToList();

            List<Category> results = new List<Category>
            {
                new Category 
                {
                    Id = -1,
                    Name = "Category Root"
                }
            };

            Action<List<Category>, int> _ChangeNameCategory = null;
            Action<List<Category>, int> ChangeNameCategory = (items, level) =>
            {
                string prefix = string.Concat(Enumerable.Repeat("--", level));
                foreach (var item in items)
                {
                    item.Name = $"{prefix} {item.Name}";
                    results.Add(item);
                    if (item.Children != null && item.Children.Count > 0)
                    {
                        _ChangeNameCategory(item.Children.ToList(), level + 1);
                    }
                }
            };

            _ChangeNameCategory = ChangeNameCategory;
            ChangeNameCategory(listitem, 0);

            return results;
        }

        // GET: Admin/Category/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ParentId"] = new SelectList(await GetItemsSelectCategory(), "Id", "Name", null);
            return View();
        }

        // POST: Admin/Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,ParentId")] Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.ParentId!.Value == -1)
                {
                    category.ParentId = null;
                }
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentId"] = new SelectList(await GetItemsSelectCategory(), "Id", "Name", category.ParentId);
            return View(category);
        }

        // GET: Admin/Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = await _context.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            ViewData["ParentId"] = new SelectList(await GetItemsSelectCategory(), "Id", "Name", category.ParentId);
            return View(category);
        }

        // POST: Admin/Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ParentId")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (category.ParentId!.Value == -1)
                    {
                        category.ParentId = null;
                    }
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            ViewData["ParentId"] = new SelectList(await GetItemsSelectCategory(), "Id", "Name", category.ParentId);
            return View(category);
        }

        // GET: Admin/Category/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Category == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Category'  is null.");
            }
            var category = await _context.Category.FindAsync(id);
            if (category != null)
            {
                _context.Category.Remove(category);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return (_context.Category?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
