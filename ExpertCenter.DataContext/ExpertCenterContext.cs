using ExpertCenter.DataContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpertCenter.DataContext;

public class ExpertCenterContext : DbContext
{
    public DbSet<ColumnType> Columns { get; set; }
    public DbSet<IntColumn> IntColumns { get; set; }
    public DbSet<VarCharColumn> VarCharColumns { get; set; }
    public DbSet<StringTextColumn> StringTextColumns { get; set; }
    public DbSet<PriceList> PriceList { get; set; }
    public DbSet<Product> Product { get; set; }

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
        modelBuilder.Entity<ColumnValueBase>(e =>
        {
            e.UseTptMappingStrategy();
            e.HasKey(x => new { x.ColumnId, x.ProductId });

            e.HasOne(x => x.Column)
                .WithMany(x => x.ColumnValues)
                .HasForeignKey(x => x.ColumnId);

            e.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId);
        });

        base.OnModelCreating(modelBuilder);
    }
}
