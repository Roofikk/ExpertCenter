using ExpertCenter.DataContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpertCenter.DataContext;

public class ExpertCenterContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<PriceList> PriceList { get; set; }

    public ExpertCenterContext()
        : base()
    {
    }

    public ExpertCenterContext(DbContextOptions<ExpertCenterContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=../ExpertCenter.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
