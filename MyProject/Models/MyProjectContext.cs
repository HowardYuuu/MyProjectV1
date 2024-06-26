﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MyProject.Models;

public partial class MyProjectContext : DbContext
{
    public MyProjectContext(DbContextOptions<MyProjectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TBrand> TBrands { get; set; }

    public virtual DbSet<TCustomer> TCustomers { get; set; }

    public virtual DbSet<TEmployee> TEmployees { get; set; }

    public virtual DbSet<TOrder> TOrders { get; set; }

    public virtual DbSet<TOrderDetail> TOrderDetails { get; set; }

    public virtual DbSet<TProduct> TProducts { get; set; }

    public virtual DbSet<TShoppingCart> TShoppingCarts { get; set; }

    public virtual DbSet<TSizeQty> TSizeQties { get; set; }

    public virtual DbSet<VproductSizeQty> VproductSizeQties { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TBrand>(entity =>
        {
            entity.HasKey(e => e.FBrandId);

            entity.ToTable("tBrand");

            entity.Property(e => e.FBrandId).HasColumnName("fBrandID");
            entity.Property(e => e.FBrandName)
                .HasMaxLength(50)
                .HasColumnName("fBrandName");
        });

        modelBuilder.Entity<TCustomer>(entity =>
        {
            entity.HasKey(e => e.FUserId);

            entity.ToTable("tCustomer");

            entity.Property(e => e.FUserId).HasColumnName("fUserID");
            entity.Property(e => e.FAccount).HasColumnName("fAccount");
            entity.Property(e => e.FEmail).HasColumnName("fEmail");
            entity.Property(e => e.FEnabled).HasColumnName("fEnabled");
            entity.Property(e => e.FImgPath).HasColumnName("fImgPath");
            entity.Property(e => e.FName)
                .HasMaxLength(50)
                .HasColumnName("fName");
            entity.Property(e => e.FPhone)
                .HasMaxLength(50)
                .HasColumnName("fPhone");
            entity.Property(e => e.FPwd).HasColumnName("fPwd");
            entity.Property(e => e.FSalt).HasColumnName("fSalt");
        });

        modelBuilder.Entity<TEmployee>(entity =>
        {
            entity.HasKey(e => e.FEmployeeId);

            entity.ToTable("tEmployee");

            entity.Property(e => e.FEmployeeId).HasColumnName("fEmployeeID");
            entity.Property(e => e.FAccount).HasColumnName("fAccount");
            entity.Property(e => e.FEmail).HasColumnName("fEmail");
            entity.Property(e => e.FName)
                .HasMaxLength(50)
                .HasColumnName("fName");
            entity.Property(e => e.FPassword).HasColumnName("fPassword");
            entity.Property(e => e.FSalt).HasColumnName("fSalt");
        });

        modelBuilder.Entity<TOrder>(entity =>
        {
            entity.HasKey(e => e.FOrderId);

            entity.ToTable("tOrders");

            entity.Property(e => e.FOrderId)
                .HasMaxLength(50)
                .HasDefaultValueSql("([dbo].[GetOrderID]())")
                .HasColumnName("fOrderID");
            entity.Property(e => e.FCusAddress)
                .HasMaxLength(50)
                .HasColumnName("fCusAddress");
            entity.Property(e => e.FCusEmail)
                .HasMaxLength(50)
                .HasColumnName("fCusEmail");
            entity.Property(e => e.FCusName)
                .HasMaxLength(50)
                .HasColumnName("fCusName");
            entity.Property(e => e.FCusPhone)
                .HasMaxLength(50)
                .HasColumnName("fCusPhone");
            entity.Property(e => e.FCustomerId).HasColumnName("fCustomerID");
            entity.Property(e => e.FEndDate)
                .HasColumnType("datetime")
                .HasColumnName("fEndDate");
            entity.Property(e => e.FId)
                .ValueGeneratedOnAdd()
                .HasColumnName("fId");
            entity.Property(e => e.FOrderDate)
                .HasColumnType("datetime")
                .HasColumnName("fOrderDate");
            entity.Property(e => e.FPaymentMethod)
                .HasMaxLength(50)
                .HasColumnName("fPaymentMethod");
            entity.Property(e => e.FStatus)
                .HasMaxLength(50)
                .HasColumnName("fStatus");
            entity.Property(e => e.FTotalAmount)
                .HasColumnType("money")
                .HasColumnName("fTotalAmount");

            entity.HasOne(d => d.FCustomer).WithMany(p => p.TOrders)
                .HasForeignKey(d => d.FCustomerId)
                .HasConstraintName("FK_tOrders_tCustomer");
        });

        modelBuilder.Entity<TOrderDetail>(entity =>
        {
            entity.HasKey(e => e.FId);

            entity.ToTable("tOrderDetails");

            entity.Property(e => e.FId).HasColumnName("fId");
            entity.Property(e => e.FOrderId)
                .HasMaxLength(50)
                .HasColumnName("fOrderID");
            entity.Property(e => e.FProductId).HasColumnName("fProductID");
            entity.Property(e => e.FQuantity).HasColumnName("fQuantity");
            entity.Property(e => e.FSize)
                .HasColumnType("decimal(3, 1)")
                .HasColumnName("fSize");
            entity.Property(e => e.FSubTotal)
                .HasColumnType("money")
                .HasColumnName("fSubTotal");
            entity.Property(e => e.FUnitPrice)
                .HasColumnType("money")
                .HasColumnName("fUnitPrice");
        });

        modelBuilder.Entity<TProduct>(entity =>
        {
            entity.HasKey(e => e.FProductId).HasName("PK_tProduct");

            entity.ToTable("tProducts");

            entity.Property(e => e.FProductId).HasColumnName("fProductID");
            entity.Property(e => e.FBrand)
                .HasMaxLength(50)
                .HasColumnName("fBrand");
            entity.Property(e => e.FGender)
                .HasMaxLength(50)
                .HasColumnName("fGender");
            entity.Property(e => e.FImgPath)
                .HasMaxLength(50)
                .HasColumnName("fImgPath");
            entity.Property(e => e.FMainColor)
                .HasMaxLength(50)
                .HasColumnName("fMainColor");
            entity.Property(e => e.FNote).HasColumnName("fNote");
            entity.Property(e => e.FNumber)
                .HasMaxLength(50)
                .HasColumnName("fNumber");
            entity.Property(e => e.FOnSale)
                .HasDefaultValueSql("((0))")
                .HasColumnName("fOnSale");
            entity.Property(e => e.FPrice)
                .HasColumnType("money")
                .HasColumnName("fPrice");
            entity.Property(e => e.FProductName)
                .HasMaxLength(50)
                .HasColumnName("fProductName");
            entity.Property(e => e.FStyle)
                .HasMaxLength(50)
                .HasColumnName("fStyle");
            entity.Property(e => e.FWidth)
                .HasMaxLength(50)
                .HasColumnName("fWidth");
        });

        modelBuilder.Entity<TShoppingCart>(entity =>
        {
            entity.HasKey(e => e.FShopCartId);

            entity.ToTable("tShoppingCart", tb => tb.HasTrigger("UpdateTotalPrice"));

            entity.Property(e => e.FShopCartId).HasColumnName("fShopCartID");
            entity.Property(e => e.FCustomerId).HasColumnName("fCustomerID");
            entity.Property(e => e.FProductId).HasColumnName("fProductID");
            entity.Property(e => e.FQuantity).HasColumnName("fQuantity");
            entity.Property(e => e.FSize)
                .HasColumnType("decimal(3, 1)")
                .HasColumnName("fSize");
            entity.Property(e => e.FSubtotal)
                .HasColumnType("money")
                .HasColumnName("fSubtotal");
            entity.Property(e => e.FUnitPrice)
                .HasColumnType("money")
                .HasColumnName("fUnitPrice");

            entity.HasOne(d => d.FCustomer).WithMany(p => p.TShoppingCarts)
                .HasForeignKey(d => d.FCustomerId)
                .HasConstraintName("FK_tShoppingCart_tCustomer");

            entity.HasOne(d => d.FProduct).WithMany(p => p.TShoppingCarts)
                .HasForeignKey(d => d.FProductId)
                .HasConstraintName("FK_tShoppingCart_tProducts");
        });

        modelBuilder.Entity<TSizeQty>(entity =>
        {
            entity.HasKey(e => e.Fid);

            entity.ToTable("tSizeQty");

            entity.Property(e => e.Fid).HasColumnName("fid");
            entity.Property(e => e.FProductId).HasColumnName("fProductID");
            entity.Property(e => e.FQuantity)
                .HasDefaultValueSql("((0))")
                .HasColumnName("fQuantity");
            entity.Property(e => e.FSize)
                .HasColumnType("decimal(3, 1)")
                .HasColumnName("fSize");

            entity.HasOne(d => d.FProduct).WithMany(p => p.TSizeQties)
                .HasForeignKey(d => d.FProductId)
                .HasConstraintName("FK_tSizeQty_tProducts");
        });

        modelBuilder.Entity<VproductSizeQty>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VProductSizeQty");

            entity.Property(e => e.價格).HasColumnType("money");
            entity.Property(e => e.名稱).HasMaxLength(50);
            entity.Property(e => e.品牌).HasMaxLength(50);
            entity.Property(e => e.圖片).HasMaxLength(50);
            entity.Property(e => e.型號).HasMaxLength(50);
            entity.Property(e => e.寬度).HasMaxLength(50);
            entity.Property(e => e.尺寸).HasColumnType("decimal(3, 1)");
            entity.Property(e => e.性別).HasMaxLength(50);
            entity.Property(e => e.產品id).HasColumnName("產品ID");
            entity.Property(e => e.顏色).HasMaxLength(50);
            entity.Property(e => e.風格).HasMaxLength(50);
        });

        OnModelCreatingGeneratedFunctions(modelBuilder);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}