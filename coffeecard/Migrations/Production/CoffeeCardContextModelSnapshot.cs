﻿// <auto-generated />
using System;
using CoffeeCard.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CoffeeCard.WebApi.Migrations.Prod
{
    [DbContext(typeof(CoffeeCardContext))]
    partial class CoffeeCardContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("dbo")
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CoffeeCard.WebApi.Models.LoginAttempt", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Time")
                        .HasColumnType("datetime2");

                    b.Property<int?>("User_Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("User_Id");

                    b.ToTable("LoginAttempts");
                });

            modelBuilder.Entity("CoffeeCard.WebApi.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ExperienceWorth")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NumberOfTickets")
                        .HasColumnType("int");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<bool>("Visible")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("CoffeeCard.WebApi.Models.ProductUserGroup", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("UserGroup")
                        .HasColumnType("int");

                    b.HasKey("ProductId", "UserGroup");

                    b.ToTable("ProductUserGroups");
                });

            modelBuilder.Entity("CoffeeCard.WebApi.Models.Programme", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ShortName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SortPriority")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Programmes");
                });

            modelBuilder.Entity("CoffeeCard.WebApi.Models.Purchase", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Completed")
                        .HasColumnType("bit");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<int>("NumberOfTickets")
                        .HasColumnType("int");

                    b.Property<string>("OrderId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<string>("ProductName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PurchasedBy_Id")
                        .HasColumnType("int");

                    b.Property<string>("TransactionId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PurchasedBy_Id");

                    b.ToTable("Purchases");
                });

            modelBuilder.Entity("CoffeeCard.WebApi.Models.Statistic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastSwipe")
                        .HasColumnType("datetime2");

                    b.Property<int>("Preset")
                        .HasColumnType("int");

                    b.Property<int>("SwipeCount")
                        .HasColumnType("int");

                    b.Property<int?>("User_Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("User_Id");

                    b.ToTable("Statistics");
                });

            modelBuilder.Entity("CoffeeCard.WebApi.Models.Ticket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateUsed")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsUsed")
                        .HasColumnType("bit");

                    b.Property<int?>("Owner_Id")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int?>("Purchase_Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Owner_Id");

                    b.HasIndex("Purchase_Id");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("CoffeeCard.WebApi.Models.Token", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("TokenHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("User_Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("User_Id");

                    b.ToTable("Tokens");
                });

            modelBuilder.Entity("CoffeeCard.WebApi.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Experience")
                        .HasColumnType("int");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PrivacyActivated")
                        .HasColumnType("bit");

                    b.Property<int?>("Programme_Id")
                        .HasColumnType("int");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserGroup")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Programme_Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CoffeeCard.WebApi.Models.Voucher", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateUsed")
                        .HasColumnType("datetime2");

                    b.Property<int?>("Product_Id")
                        .HasColumnType("int");

                    b.Property<int?>("User_Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Product_Id");

                    b.HasIndex("User_Id");

                    b.ToTable("Vouchers");
                });

            modelBuilder.Entity("CoffeeCard.WebApi.Models.LoginAttempt", b =>
                {
                    b.HasOne("CoffeeCard.WebApi.Models.User", "User")
                        .WithMany("LoginAttempts")
                        .HasForeignKey("User_Id");
                });

            modelBuilder.Entity("CoffeeCard.WebApi.Models.ProductUserGroup", b =>
                {
                    b.HasOne("CoffeeCard.WebApi.Models.Product", "Product")
                        .WithMany("ProductUserGroup")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CoffeeCard.WebApi.Models.Purchase", b =>
                {
                    b.HasOne("CoffeeCard.WebApi.Models.User", "PurchasedBy")
                        .WithMany("Purchases")
                        .HasForeignKey("PurchasedBy_Id");
                });

            modelBuilder.Entity("CoffeeCard.WebApi.Models.Statistic", b =>
                {
                    b.HasOne("CoffeeCard.WebApi.Models.User", "User")
                        .WithMany("Statistics")
                        .HasForeignKey("User_Id");
                });

            modelBuilder.Entity("CoffeeCard.WebApi.Models.Ticket", b =>
                {
                    b.HasOne("CoffeeCard.WebApi.Models.User", "Owner")
                        .WithMany("Tickets")
                        .HasForeignKey("Owner_Id");

                    b.HasOne("CoffeeCard.WebApi.Models.Purchase", "Purchase")
                        .WithMany("Tickets")
                        .HasForeignKey("Purchase_Id");
                });

            modelBuilder.Entity("CoffeeCard.WebApi.Models.Token", b =>
                {
                    b.HasOne("CoffeeCard.WebApi.Models.User", "User")
                        .WithMany("Tokens")
                        .HasForeignKey("User_Id");
                });

            modelBuilder.Entity("CoffeeCard.WebApi.Models.User", b =>
                {
                    b.HasOne("CoffeeCard.WebApi.Models.Programme", "Programme")
                        .WithMany("Users")
                        .HasForeignKey("Programme_Id");
                });

            modelBuilder.Entity("CoffeeCard.WebApi.Models.Voucher", b =>
                {
                    b.HasOne("CoffeeCard.WebApi.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("Product_Id");

                    b.HasOne("CoffeeCard.WebApi.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("User_Id");
                });
#pragma warning restore 612, 618
        }
    }
}
