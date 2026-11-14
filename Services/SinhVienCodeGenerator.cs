using System.Data;
using System.Globalization;
using DemoMVC.Data;
using DemoMVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DemoMVC.Services;

public class SinhVienCodeGenerator(AppDbContext context)
{
    public async Task CreateAsync(SinhVien sinhVien, CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var lockResource = string.Create(
            CultureInfo.InvariantCulture,
            $"SinhVienCode:{sinhVien.NamNhapHoc:D4}:{sinhVien.MaHeDaoTao}:{sinhVien.MaNganh}");

        await using var command = context.Database.GetDbConnection().CreateCommand();
        command.Transaction = transaction.GetDbTransaction();
        command.CommandText = """
            DECLARE @result int;
            EXEC @result = sp_getapplock
                @Resource = @resource,
                @LockMode = 'Exclusive',
                @LockOwner = 'Transaction',
                @LockTimeout = 10000;
            SELECT @result;
            """;

        var resourceParameter = command.CreateParameter();
        resourceParameter.ParameterName = "@resource";
        resourceParameter.DbType = DbType.String;
        resourceParameter.Size = 255;
        resourceParameter.Value = lockResource;
        command.Parameters.Add(resourceParameter);

        var lockResultValue = await command.ExecuteScalarAsync(cancellationToken);
        if (lockResultValue is not int lockResult)
            throw new InvalidOperationException("SQL Server không trả về kết quả khóa bộ đếm mã sinh viên.");

        if (lockResult < 0)
            throw new TimeoutException("Không thể khóa bộ đếm mã sinh viên trong thời gian cho phép.");

        var namHaiChuSo = (sinhVien.NamNhapHoc % 100).ToString("D2", CultureInfo.InvariantCulture);
        var maSinhVienPrefix = $"{namHaiChuSo}{sinhVien.MaHeDaoTao}{sinhVien.MaNganh}";
        var maSinhVienCuoi = await context.SinhViens
            .Where(item => item.MaSinhVien.StartsWith(maSinhVienPrefix)
                && item.MaSinhVien.Length == 10)
            .OrderByDescending(item => item.MaSinhVien)
            .Select(item => item.MaSinhVien)
            .FirstOrDefaultAsync(cancellationToken);

        var soThuTuDaCo = 0;
        if (maSinhVienCuoi is not null
            && !int.TryParse(
                maSinhVienCuoi.AsSpan(6, 4),
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out soThuTuDaCo))
        {
            throw new InvalidOperationException(
                $"Mã sinh viên hiện có '{maSinhVienCuoi}' không đúng định dạng 10 chữ số.");
        }

        var sequence = await context.SinhVienCodeSequences.SingleOrDefaultAsync(
            item => item.NamNhapHoc == sinhVien.NamNhapHoc
                && item.MaHeDaoTao == sinhVien.MaHeDaoTao
                && item.MaNganh == sinhVien.MaNganh,
            cancellationToken);

        var soThuTuCuoi = sequence is null
            ? soThuTuDaCo
            : Math.Max(sequence.SoThuTuCuoi, soThuTuDaCo);

        if (soThuTuCuoi >= 9999)
            throw new SinhVienCodeSequenceExhaustedException();

        soThuTuCuoi++;

        if (sequence is null)
        {
            sequence = new SinhVienCodeSequence
            {
                NamNhapHoc = sinhVien.NamNhapHoc,
                MaHeDaoTao = sinhVien.MaHeDaoTao,
                MaNganh = sinhVien.MaNganh,
                SoThuTuCuoi = soThuTuCuoi
            };
            context.SinhVienCodeSequences.Add(sequence);
        }
        else
        {
            sequence.SoThuTuCuoi = soThuTuCuoi;
        }

        sinhVien.MaSinhVien = string.Create(
            CultureInfo.InvariantCulture,
            $"{maSinhVienPrefix}{soThuTuCuoi:D4}");

        context.SinhViens.Add(sinhVien);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
