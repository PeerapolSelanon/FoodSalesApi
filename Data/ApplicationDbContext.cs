using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<FoodSales> FoodSales { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FoodSales>()
            .Property(p => p.UnitPrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<FoodSales>()
            .Property(p => p.TotalPrice)
            .HasColumnType("decimal(18,2)");
    }
}