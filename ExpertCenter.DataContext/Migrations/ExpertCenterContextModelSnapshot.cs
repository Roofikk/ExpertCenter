﻿// <auto-generated />
using ExpertCenter.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ExpertCenter.DataContext.Migrations
{
    [DbContext(typeof(ExpertCenterContext))]
    partial class ExpertCenterContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.7");

            modelBuilder.Entity("ExpertCenter.DataContext.Entities.Column", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ColumnTypeId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<int>("PriceListId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ColumnTypeId");

                    b.HasIndex("PriceListId");

                    b.ToTable("Columns");
                });

            modelBuilder.Entity("ExpertCenter.DataContext.Entities.ColumnType", b =>
                {
                    b.Property<string>("ColumnTypeId")
                        .HasColumnType("TEXT");

                    b.Property<string>("DisplayColumnName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ColumnTypeId");

                    b.ToTable("ColumnTypes");
                });

            modelBuilder.Entity("ExpertCenter.DataContext.Entities.ColumnValueBase", b =>
                {
                    b.Property<int>("ColumnId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ProductId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ColumnId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("ColumnValueBase");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("ExpertCenter.DataContext.Entities.PriceList", b =>
                {
                    b.Property<int>("PriceListId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.HasKey("PriceListId");

                    b.ToTable("PriceList");
                });

            modelBuilder.Entity("ExpertCenter.DataContext.Entities.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Article")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("PriceListId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ProductId");

                    b.HasIndex("PriceListId");

                    b.ToTable("Product");
                });

            modelBuilder.Entity("ExpertCenter.DataContext.Entities.IntColumn", b =>
                {
                    b.HasBaseType("ExpertCenter.DataContext.Entities.ColumnValueBase");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.ToTable("IntColumns");
                });

            modelBuilder.Entity("ExpertCenter.DataContext.Entities.StringTextColumn", b =>
                {
                    b.HasBaseType("ExpertCenter.DataContext.Entities.ColumnValueBase");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.ToTable("StringTextColumns");
                });

            modelBuilder.Entity("ExpertCenter.DataContext.Entities.VarCharColumn", b =>
                {
                    b.HasBaseType("ExpertCenter.DataContext.Entities.ColumnValueBase");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.ToTable("VarCharColumns");
                });

            modelBuilder.Entity("ExpertCenter.DataContext.Entities.Column", b =>
                {
                    b.HasOne("ExpertCenter.DataContext.Entities.ColumnType", "ColumnType")
                        .WithMany("Columns")
                        .HasForeignKey("ColumnTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ExpertCenter.DataContext.Entities.PriceList", "PriceList")
                        .WithMany("Columns")
                        .HasForeignKey("PriceListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ColumnType");

                    b.Navigation("PriceList");
                });

            modelBuilder.Entity("ExpertCenter.DataContext.Entities.ColumnValueBase", b =>
                {
                    b.HasOne("ExpertCenter.DataContext.Entities.Column", "Column")
                        .WithMany("ColumnValues")
                        .HasForeignKey("ColumnId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ExpertCenter.DataContext.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Column");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("ExpertCenter.DataContext.Entities.Product", b =>
                {
                    b.HasOne("ExpertCenter.DataContext.Entities.PriceList", "PriceList")
                        .WithMany("Products")
                        .HasForeignKey("PriceListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PriceList");
                });

            modelBuilder.Entity("ExpertCenter.DataContext.Entities.IntColumn", b =>
                {
                    b.HasOne("ExpertCenter.DataContext.Entities.ColumnValueBase", null)
                        .WithOne()
                        .HasForeignKey("ExpertCenter.DataContext.Entities.IntColumn", "ColumnId", "ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ExpertCenter.DataContext.Entities.StringTextColumn", b =>
                {
                    b.HasOne("ExpertCenter.DataContext.Entities.ColumnValueBase", null)
                        .WithOne()
                        .HasForeignKey("ExpertCenter.DataContext.Entities.StringTextColumn", "ColumnId", "ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ExpertCenter.DataContext.Entities.VarCharColumn", b =>
                {
                    b.HasOne("ExpertCenter.DataContext.Entities.ColumnValueBase", null)
                        .WithOne()
                        .HasForeignKey("ExpertCenter.DataContext.Entities.VarCharColumn", "ColumnId", "ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ExpertCenter.DataContext.Entities.Column", b =>
                {
                    b.Navigation("ColumnValues");
                });

            modelBuilder.Entity("ExpertCenter.DataContext.Entities.ColumnType", b =>
                {
                    b.Navigation("Columns");
                });

            modelBuilder.Entity("ExpertCenter.DataContext.Entities.PriceList", b =>
                {
                    b.Navigation("Columns");

                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
