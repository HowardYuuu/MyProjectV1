﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using MyProject.Models;

namespace MyProject.Models
{
    public partial class MyProjectContext
    {

        [DbFunction("GetOrderID", "dbo")]
        public static string GetOrderID()
        {
            throw new NotSupportedException("This method can only be called from Entity Framework Core queries");
        }

        protected void OnModelCreatingGeneratedFunctions(ModelBuilder modelBuilder)
        {
        }
    }
}