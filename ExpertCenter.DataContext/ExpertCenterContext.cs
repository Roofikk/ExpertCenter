using ExpertCenter.DataContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpertCenter.DataContext;

public class ExpertCenterContext : DbContext
{
    public DbSet<ColumnType> ColumnTypes { get; set; }
    public DbSet<Column> Columns { get; set; }
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
                .WithMany(x => x.ColumnValues)
                .HasForeignKey(x => x.ProductId);
        });

        modelBuilder.Entity<ColumnType>(e =>
        {
            e.HasKey(x => x.ColumnTypeId);
            e.Property(x => x.ColumnTypeId).ValueGeneratedNever();

            e.HasMany(x => x.Columns)
                .WithOne(x => x.ColumnType)
                .HasForeignKey(x => x.ColumnTypeId);
        });

        base.OnModelCreating(modelBuilder);
    }
}
