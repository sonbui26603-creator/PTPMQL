
using DemoMVC.Data;
using DemoMVC.Models;
using DemoMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoMVC.Controllers
{
    [Authorize]
    public class KhoaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KhoaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Khoa
        public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 10)
        {
            search = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
            page = page < 1 ? 1 : page;
            pageSize = pageSize is >= 5 and <= 50 ? pageSize : 10;

            var query = _context.Khoas
                .Include(k => k.SinhViens)
                .AsQueryable();

            if (search != null)
            {
                if (int.TryParse(search, out var maKhoa))
                {
                    query = query.Where(k =>
                        k.TenKhoa.Contains(search) || k.MaKhoa == maKhoa);
                }
                else
                {
                    query = query.Where(k => k.TenKhoa.Contains(search));
                }
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (totalPages > 0 && page > totalPages)
                page = totalPages;

            var khoas = await query
                .OrderBy(k => k.MaKhoa)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new PagedResult<Khoa>
            {
                Items = khoas,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                SearchTerm = search
            };

            return View(model);
        }

        // GET: Khoa/Create
        [Authorize(Roles = IdentitySeeder.AdminRole)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Khoa/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = IdentitySeeder.AdminRole)]
        public async Task<IActionResult> Create(Khoa khoa)
        {
            if (!ModelState.IsValid)
                return View(khoa);

            _context.Khoas.Add(khoa);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Khoa/Edit/5
        [Authorize(Roles = IdentitySeeder.AdminRole)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var khoa = await _context.Khoas.FindAsync(id);

            if (khoa == null)
                return NotFound();

            return View(khoa);
        }

        // POST: Khoa/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = IdentitySeeder.AdminRole)]
        public async Task<IActionResult> Edit(int id, Khoa khoa)
        {
            if (id != khoa.MaKhoa)
                return NotFound();

            if (!ModelState.IsValid)
                return View(khoa);

            _context.Update(khoa);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Khoa/Delete/5
        [Authorize(Roles = IdentitySeeder.AdminRole)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var khoa = await _context.Khoas
                .Include(k => k.SinhViens)
                .FirstOrDefaultAsync(k => k.MaKhoa == id);

            if (khoa == null)
                return NotFound();

            return View(khoa);
        }

        // POST: Khoa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = IdentitySeeder.AdminRole)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var khoa = await _context.Khoas
                .Include(k => k.SinhViens)
                .FirstOrDefaultAsync(k => k.MaKhoa == id);

            if (khoa == null)
                return NotFound();

            if (khoa.SinhViens.Any())
            {
                TempData["Error"] =
                    "Không thể xóa khoa vì vẫn còn sinh viên thuộc khoa này.";

                return RedirectToAction(nameof(Index));
            }

            _context.Khoas.Remove(khoa);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
