using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class FoodSalesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ExcelImportService _excelImportService;

    public FoodSalesController(ApplicationDbContext context, ExcelImportService excelImportService)
    {
        _context = context;
        _excelImportService = excelImportService;
    }

    // GET: api/foodsales
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FoodSales>>> GetAll(
        [FromQuery] string? region = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = "asc")
    {
        var query = _context.FoodSales.AsQueryable();

        // Filtering
        if (!string.IsNullOrEmpty(region))
            query = query.Where(x => x.Region == region);

        // Sorting
        if (!string.IsNullOrEmpty(sortBy))
        {
            query = sortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(x => EF.Property<object>(x, sortBy))
                : query.OrderBy(x => EF.Property<object>(x, sortBy));
        }

        return await query.ToListAsync();
    }

    // GET: api/foodsales/5
    [HttpGet("{id}")]
    public async Task<ActionResult<FoodSales>> GetById(int id)
    {
        var order = await _context.FoodSales.FindAsync(id);
        if (order == null)
            return NotFound();

        return order;
    }

    // POST: api/foodsales
    [HttpPost]
    public async Task<ActionResult<FoodSales>> Create(FoodSales order)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.FoodSales.Add(order);
        await _context.SaveChangesAsync();

        // Return 201 Created with location header
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    // PUT: api/foodsales/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, FoodSales order)
    {
        if (id != order.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await FoodSalesExists(id))
                return NotFound();
            throw;
        }

        return NoContent(); // 204 No Content
    }

    // DELETE: api/foodsales/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var order = await _context.FoodSales.FindAsync(id);
        if (order == null)
            return NotFound();

        _context.FoodSales.Remove(order);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // POST: api/foodsales/import
    [HttpPost("import")]
    public async Task<IActionResult> ImportExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        var filePath = Path.GetTempFileName();
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        await _excelImportService.ImportExcelData(filePath);
        System.IO.File.Delete(filePath);

        return Ok("Data imported successfully");
    }

    private async Task<bool> FoodSalesExists(int id)
    {
        return await _context.FoodSales.AnyAsync(e => e.Id == id);
    }
}