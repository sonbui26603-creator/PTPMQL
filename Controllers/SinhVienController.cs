using DemoMVC.Data;
using DemoMVC.Models;
using DemoMVC.Services;
using DemoMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DemoMVC.Controllers;

public class SinhVienController(AppDbContext context, SinhVienCodeGenerator codeGenerator) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var sinhViens = await context.SinhViens
            .AsNoTracking()
            .Include(sinhVien => sinhVien.Khoa)
            .OrderBy(sinhVien => sinhVien.MaSinhVien)
            .ToListAsync(cancellationToken);

        return View(sinhViens);
    }

    public async Task<IActionResult> Details(string? id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        var sinhVien = await context.SinhViens
            .AsNoTracking()
            .Include(item => item.Khoa)
            .SingleOrDefaultAsync(item => item.MaSinhVien == id, cancellationToken);

        return sinhVien is null ? NotFound() : View(sinhVien);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var viewModel = new SinhVienCreateViewModel();
        await PopulateKhoaOptionsAsync(viewModel, cancellationToken);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        SinhVienCreateViewModel viewModel,
        CancellationToken cancellationToken)
    {
        if (ModelState.IsValid
            && !await context.Khoas.AnyAsync(khoa => khoa.MaKhoa == viewModel.MaKhoa, cancellationToken))
        {
            ModelState.AddModelError(nameof(viewModel.MaKhoa), "Khoa được chọn không tồn tại.");
        }

        if (!ModelState.IsValid)
        {
            await PopulateKhoaOptionsAsync(viewModel, cancellationToken);
            return View(viewModel);
        }

        var sinhVien = new SinhVien
        {
            HoTen = viewModel.HoTen.Trim(),
            NamNhapHoc = viewModel.NamNhapHoc,
            MaHeDaoTao = viewModel.MaHeDaoTao,
            MaNganh = viewModel.MaNganh,
            MaKhoa = viewModel.MaKhoa
        };

        try
        {
            await codeGenerator.CreateAsync(sinhVien, cancellationToken);
        }
        catch (SinhVienCodeSequenceExhaustedException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await PopulateKhoaOptionsAsync(viewModel, cancellationToken);
            return View(viewModel);
        }
        catch (TimeoutException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await PopulateKhoaOptionsAsync(viewModel, cancellationToken);
            return View(viewModel);
        }

        TempData["ThongBao"] = $"Đã thêm sinh viên với mã {sinhVien.MaSinhVien}.";
        return RedirectToAction(nameof(Details), new { id = sinhVien.MaSinhVien });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string? id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        var sinhVien = await context.SinhViens
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.MaSinhVien == id, cancellationToken);

        if (sinhVien is null)
            return NotFound();

        var viewModel = new SinhVienEditViewModel
        {
            MaSinhVien = sinhVien.MaSinhVien,
            HoTen = sinhVien.HoTen,
            MaKhoa = sinhVien.MaKhoa
        };
        await PopulateKhoaOptionsAsync(viewModel, cancellationToken);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        string id,
        SinhVienEditViewModel viewModel,
        CancellationToken cancellationToken)
    {
        if (id != viewModel.MaSinhVien)
            return NotFound();

        if (ModelState.IsValid
            && !await context.Khoas.AnyAsync(khoa => khoa.MaKhoa == viewModel.MaKhoa, cancellationToken))
        {
            ModelState.AddModelError(nameof(viewModel.MaKhoa), "Khoa được chọn không tồn tại.");
        }

        var sinhVien = await context.SinhViens
            .SingleOrDefaultAsync(item => item.MaSinhVien == id, cancellationToken);

        if (sinhVien is null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            await PopulateKhoaOptionsAsync(viewModel, cancellationToken);
            return View(viewModel);
        }

        sinhVien.HoTen = viewModel.HoTen.Trim();
        sinhVien.MaKhoa = viewModel.MaKhoa;
        await context.SaveChangesAsync(cancellationToken);

        TempData["ThongBao"] = "Đã cập nhật thông tin sinh viên.";
        return RedirectToAction(nameof(Details), new { id = sinhVien.MaSinhVien });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string? id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        var sinhVien = await context.SinhViens
            .AsNoTracking()
            .Include(item => item.Khoa)
            .SingleOrDefaultAsync(item => item.MaSinhVien == id, cancellationToken);

        return sinhVien is null ? NotFound() : View(sinhVien);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id, CancellationToken cancellationToken)
    {
        var sinhVien = await context.SinhViens.FindAsync([id], cancellationToken);

        if (sinhVien is null)
            return NotFound();

        context.SinhViens.Remove(sinhVien);
        await context.SaveChangesAsync(cancellationToken);

        TempData["ThongBao"] = "Đã xóa sinh viên.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateKhoaOptionsAsync(
        SinhVienCreateViewModel viewModel,
        CancellationToken cancellationToken)
    {
        viewModel.DanhSachKhoa = await GetKhoaOptionsAsync(viewModel.MaKhoa, cancellationToken);
    }

    private async Task PopulateKhoaOptionsAsync(
        SinhVienEditViewModel viewModel,
        CancellationToken cancellationToken)
    {
        viewModel.DanhSachKhoa = await GetKhoaOptionsAsync(viewModel.MaKhoa, cancellationToken);
    }

    private async Task<List<SelectListItem>> GetKhoaOptionsAsync(
        string selectedMaKhoa,
        CancellationToken cancellationToken)
    {
        return await context.Khoas
            .AsNoTracking()
            .OrderBy(khoa => khoa.TenKhoa)
            .Select(khoa => new SelectListItem
            {
                Value = khoa.MaKhoa,
                Text = $"{khoa.TenKhoa} ({khoa.MaKhoa})",
                Selected = khoa.MaKhoa == selectedMaKhoa
            })
            .ToListAsync(cancellationToken);
    }
}
