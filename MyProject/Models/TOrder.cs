﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace MyProject.Models;

public partial class TOrder
{
    public int FId { get; set; }

    public string FOrderId { get; set; }

    public int? FCustomerId { get; set; }

    public string FCusName { get; set; }

    public string FCusEmail { get; set; }

    public string FCusPhone { get; set; }

    public string FCusAddress { get; set; }

    public DateTime? FOrderDate { get; set; }

    public DateTime? FEndDate { get; set; }

    public decimal? FTotalAmount { get; set; }

    public string FPaymentMethod { get; set; }

    public string FStatus { get; set; }

    public virtual TCustomer FCustomer { get; set; }
}