
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
        public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 10)
        {
            search = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
            page = page < 1 ? 1 : page;
            pageSize = pageSize is >= 5 and <= 50 ? pageSize : 10;

            var query = _context.SinhViens
                .Include(s => s.Khoa)
                .AsQueryable();

            if (search != null)
            {
                if (int.TryParse(search, out var id))
                {
                    query = query.Where(s =>
                        s.HoTen.Contains(search) ||
                        s.Email.Contains(search) ||
                        s.Id == id ||
                        (s.Khoa != null && s.Khoa.TenKhoa.Contains(search)));
                }
                else
                {
                    query = query.Where(s =>
                        s.HoTen.Contains(search) ||
                        s.Email.Contains(search) ||
                        (s.Khoa != null && s.Khoa.TenKhoa.Contains(search)));
                }
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (totalPages > 0 && page > totalPages)
                page = totalPages;

            var sinhViens = await query
                .OrderBy(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new PagedResult<SinhVien>
            {
                Items = sinhViens,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                SearchTerm = search
            };

            return View(model);
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
