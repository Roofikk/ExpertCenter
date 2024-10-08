﻿using ExpertCenter.DataContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpertCenter.DataContext;

/// <summary>
/// Code First через EF Core миграции
/// </summary>
public class ExpertCenterContext : DbContext
{
    public DbSet<ColumnType> ColumnTypes { get; set; }
    public DbSet<Column> Columns { get; set; }
    public DbSet<IntColumn> IntColumns { get; set; }
    public DbSet<VarCharColumn> VarCharColumns { get; set; }
    public DbSet<StringTextColumn> StringTextColumns { get; set; }
    public DbSet<PriceList> PriceLists { get; set; }
    public DbSet<Product> Products { get; set; }

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
            optionsBuilder.UseSqlServer(@"Server=RUFIKDESKTOP;Database=ExpertCenter;User=sa;Password=root1234;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ColumnValueBase>(e =>
        {
            e.UseTpcMappingStrategy();
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

        modelBuilder.Entity<Product>(e =>
        {
            e.ToTable(tb => tb.UseSqlOutputClause(false));
            e.HasIndex(x => x.Article).HasDatabaseName("IX_Products_Article");
        });

        modelBuilder.Entity<PriceList>(e =>
        {
            e.HasKey(x => x.PriceListId);
            e.Property(x => x.PriceListId).ValueGeneratedOnAdd();

            e.HasMany(x => x.Columns)
                .WithMany(x => x.PriceLists)
                .UsingEntity(
                    "PriceListColumns",
                    x => x.HasOne(typeof(Column)).WithMany().HasForeignKey("ColumnId"),
                    x => x.HasOne(typeof(PriceList)).WithMany().HasForeignKey("PriceListId"),
                    j => j.HasKey("ColumnId", "PriceListId"));
        });
    }
}
