using OfficeOpenXml;
using System.Collections.Generic;

public class ExcelImportService
{
    private readonly ApplicationDbContext _context;

    public ExcelImportService(ApplicationDbContext context)
    {
        _context = context;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task ImportExcelData(string filePath)
    {
        using var package = new ExcelPackage(new FileInfo(filePath));
        var worksheet = package.Workbook.Worksheets[0];
        int rowCount = worksheet.Dimension.Rows;

        // เริ่มจากแถวที่ 2 (ข้ามส่วนหัว)
        for (int row = 2; row <= rowCount; row++)
        {
            var order = new FoodSales
            {
                OrderDate = DateTime.Parse(worksheet.Cells[row, 1].Value?.ToString() ?? DateTime.Now.ToString()),
                Region = worksheet.Cells[row, 2].Value?.ToString() ?? "",
                City = worksheet.Cells[row, 3].Value?.ToString() ?? "",
                Category = worksheet.Cells[row, 4].Value?.ToString() ?? "",
                Product = worksheet.Cells[row, 5].Value?.ToString() ?? "",
                Quantity = int.Parse(worksheet.Cells[row, 6].Value?.ToString() ?? "0"),
                UnitPrice = decimal.Parse(worksheet.Cells[row, 7].Value?.ToString() ?? "0"),
                TotalPrice = decimal.Parse(worksheet.Cells[row, 8].Value?.ToString() ?? "0")
            };
            _context.FoodSales.Add(order);
        }
        await _context.SaveChangesAsync();
    }
}