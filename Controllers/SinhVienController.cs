
using DemoMVC.Data;
using DemoMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DemoMVC.Controllers
{
    public class SinhVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SinhVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Nạp danh sách khoa cho dropdown
        private async Task NapDanhSachKhoa(int? maKhoa = null)
        {
            var khoas = await _context.Khoas
                .OrderBy(k => k.TenKhoa)
                .ToListAsync();

            ViewBag.MaKhoa = new SelectList(
                khoas, "MaKhoa", "TenKhoa", maKhoa);
        }

        // GET: SinhVien
        public async Task<IActionResult> Index()
        {
            var sinhViens = await _context.SinhViens
                .Include(s => s.Khoa)
                .OrderBy(s => s.Id)
                .ToListAsync();

            return View(sinhViens);
        }

        // GET: SinhVien/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var sinhVien = await _context.SinhViens
                .Include(s => s.Khoa)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sinhVien == null)
                return NotFound();

            return View(sinhVien);
        }

        // GET: SinhVien/Create
        public async Task<IActionResult> Create()
        {
            await NapDanhSachKhoa();
            return View();
        }

        // POST: SinhVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SinhVien sinhVien)
        {
            if (!await _context.Khoas.AnyAsync(
                k => k.MaKhoa == sinhVien.MaKhoa))
            {
                ModelState.AddModelError(
                    "MaKhoa", "Khoa được chọn không tồn tại.");
            }

            if (!ModelState.IsValid)
            {
                await NapDanhSachKhoa(sinhVien.MaKhoa);
                return View(sinhVien);
            }

            _context.SinhViens.Add(sinhVien);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: SinhVien/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var sinhVien = await _context.SinhViens.FindAsync(id);

            if (sinhVien == null)
                return NotFound();

            await NapDanhSachKhoa(sinhVien.MaKhoa);
            return View(sinhVien);
        }

        // POST: SinhVien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, SinhVien sinhVien)
        {
            if (id != sinhVien.Id)
                return NotFound();

            if (!await _context.Khoas.AnyAsync(
                k => k.MaKhoa == sinhVien.MaKhoa))
            {
                ModelState.AddModelError(
                    "MaKhoa", "Khoa được chọn không tồn tại.");
            }

            if (!ModelState.IsValid)
            {
                await NapDanhSachKhoa(sinhVien.MaKhoa);
                return View(sinhVien);
            }

            _context.Update(sinhVien);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: SinhVien/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var sinhVien = await _context.SinhViens
                .Include(s => s.Khoa)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sinhVien == null)
                return NotFound();

            return View(sinhVien);
        }

        // POST: SinhVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sinhVien = await _context.SinhViens.FindAsync(id);

            if (sinhVien == null)
                return NotFound();

            _context.SinhViens.Remove(sinhVien);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
