﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using riusco_mvc.Data;

#nullable disable

namespace riuscomvc.Migrations
{
    [DbContext(typeof(MainDbContext))]
    partial class MainDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("riusco_mvc.Models.ProductDTO", b =>
                {
                    b.Property<int>("ProductID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<string>("Image")
                        .HasColumnType("longtext");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("LastUpdate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<int>("OwnerID")
                        .HasColumnType("int");

                    b.HasKey("ProductID");

                    b.HasIndex("OwnerID");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("riusco_mvc.Models.TransactionDTO", b =>
                {
                    b.Property<int>("TransactionID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("BuyerID")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastUpdate")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("OwnerID")
                        .HasColumnType("int");

                    b.Property<int?>("ProductID")
                        .HasColumnType("int");

                    b.Property<string>("State")
                        .HasColumnType("longtext");

                    b.HasKey("TransactionID");

                    b.HasIndex("BuyerID");

                    b.HasIndex("OwnerID");

                    b.HasIndex("ProductID");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("riusco_mvc.Models.UserDTO", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ApiKey")
                        .HasColumnType("longtext");

                    b.Property<int>("Balance")
                        .HasColumnType("int");

                    b.Property<string>("City")
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .HasColumnType("longtext");

                    b.Property<string>("Image")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<string>("Password")
                        .HasColumnType("longtext");

                    b.Property<string>("Salt")
                        .HasColumnType("longtext");

                    b.HasKey("UserID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("riusco_mvc.Models.ProductDTO", b =>
                {
                    b.HasOne("riusco_mvc.Models.UserDTO", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("riusco_mvc.Models.TransactionDTO", b =>
                {
                    b.HasOne("riusco_mvc.Models.UserDTO", "Buyer")
                        .WithMany()
                        .HasForeignKey("BuyerID");

                    b.HasOne("riusco_mvc.Models.UserDTO", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerID");

                    b.HasOne("riusco_mvc.Models.ProductDTO", "Product")
                        .WithMany()
                        .HasForeignKey("ProductID");

                    b.Navigation("Buyer");

                    b.Navigation("Owner");

                    b.Navigation("Product");
                });
#pragma warning restore 612, 618
        }
    }
}