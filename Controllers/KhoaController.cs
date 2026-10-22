
using DemoMVC.Data;
using DemoMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoMVC.Controllers
{
    public class KhoaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KhoaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Khoa
        public async Task<IActionResult> Index()
        {
            var khoas = await _context.Khoas
                .Include(k => k.SinhViens)
                .OrderBy(k => k.MaKhoa)
                .ToListAsync();

            return View(khoas);
        }

        // GET: Khoa/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Khoa/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Khoa khoa)
        {
            if (!ModelState.IsValid)
                return View(khoa);

            _context.Khoas.Add(khoa);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Khoa/Edit/5
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
