﻿// <auto-generated />
using EShopBE.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EShopBE.Migrations
{
    [DbContext(typeof(ApplicationDBContext))]
    partial class ApplicationDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("EShopBE.models.Stock", b =>
                {
                    b.Property<string>("CodeSKU")
                        .HasColumnType("varchar(255)")
                        .HasColumnName("codeSKU");

                    b.Property<string>("Barcode")
                        .HasColumnType("text")
                        .HasColumnName("barcode");

                    b.Property<string>("Color")
                        .HasColumnType("text")
                        .HasColumnName("color");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("Group")
                        .HasColumnType("text")
                        .HasColumnName("group");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("text")
                        .HasColumnName("imageUrl");

                    b.Property<string>("IsHide")
                        .HasColumnType("text")
                        .HasColumnName("isHide");

                    b.Property<ulong>("IsParent")
                        .HasColumnType("bit")
                        .HasColumnName("isParent");

                    b.Property<string>("ManagerBy")
                        .HasColumnType("text")
                        .HasColumnName("manager_by");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<long>("Price")
                        .HasColumnType("bigint")
                        .HasColumnName("price");

                    b.Property<long>("Sell")
                        .HasColumnType("bigint")
                        .HasColumnName("sell");

                    b.Property<string>("Status")
                        .HasColumnType("text")
                        .HasColumnName("status");

                    b.Property<string>("Type")
                        .HasColumnType("text")
                        .HasColumnName("type");

                    b.Property<string>("Unit")
                        .HasColumnType("text")
                        .HasColumnName("unit");

                    b.HasKey("CodeSKU");

                    b.ToTable("Stocks");
                });
#pragma warning restore 612, 618
        }
    }
}
