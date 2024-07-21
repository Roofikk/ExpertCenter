﻿// <auto-generated />
using ExpertCenter.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ExpertCenter.DataContext.Migrations
{
    [DbContext(typeof(ExpertCenterContext))]
    [Migration("20240721185627_RenameTables")]
    partial class RenameTables
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ExpertCenter.DataContext.Entities.Column", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ColumnTypeId")
                        .IsRequired()
                        .HasColumnType("varchar(24)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("ColumnTypeId");

                    b.ToTable("Columns");
                });

            modelBuilder.Entity("ExpertCenter.DataContext.Entities.ColumnType", b =>
                {
                    b.Property<string>("ColumnTypeId")
                        .HasColumnType("varchar(24)");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.HasKey("ColumnTypeId");

                    b.ToTable("ColumnTypes");
                });

            modelBuilder.Entity("ExpertCenter.DataContext.Entities.ColumnValueBase", b =>
                {
                    b.Property<int>("ColumnId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("ColumnId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable((string)null);

                    b.UseTpcMappingStrategy();
                });

            modelBuilder.Entity("ExpertCenter.DataContext.Entities.PriceList", b =>
                {
                    b.Property<int>("PriceListId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PriceListId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.HasKey("PriceListId");

                    b.ToTable("PriceLists");
                });

            modelBuilder.Entity("ExpertCenter.DataContext.Entities.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"));

                    b.Property<int>("Article")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<int>("PriceListId")
                        .HasColumnType("int");

                    b.HasKey("ProductId");

                    b.HasIndex("PriceListId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("PriceListColumns", b =>
                {
                    b.Property<int>("ColumnId")
                        .HasColumnType("int");

                    b.Property<int>("PriceListId")
                        .HasColumnType("int");

                    b.HasKey("ColumnId", "PriceListId");

                    b.HasIndex("PriceListId");

                    b.ToTable("PriceListColumns");
                });

            modelBuilder.Entity("ExpertCenter.DataContext.Entities.IntColumn", b =>
                {
                    b.HasBaseType("ExpertCenter.DataContext.Entities.ColumnValueBase");

                    b.Property<int>("Value")
                        .HasColumnType("int");

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

                    b.Navigation("ColumnType");
                });

            modelBuilder.Entity("ExpertCenter.DataContext.Entities.ColumnValueBase", b =>
                {
                    b.HasOne("ExpertCenter.DataContext.Entities.Column", "Column")
                        .WithMany("ColumnValues")
                        .HasForeignKey("ColumnId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ExpertCenter.DataContext.Entities.Product", "Product")
                        .WithMany("ColumnValues")
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

            modelBuilder.Entity("PriceListColumns", b =>
                {
                    b.HasOne("ExpertCenter.DataContext.Entities.Column", null)
                        .WithMany()
                        .HasForeignKey("ColumnId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ExpertCenter.DataContext.Entities.PriceList", null)
                        .WithMany()
                        .HasForeignKey("PriceListId")
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
                    b.Navigation("Products");
                });

            modelBuilder.Entity("ExpertCenter.DataContext.Entities.Product", b =>
                {
                    b.Navigation("ColumnValues");
                });
#pragma warning restore 612, 618
        }
    }
}
