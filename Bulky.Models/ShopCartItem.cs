﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bulky.Models
{
    public class ShopCartItem
    {
        public int Id { get; set; }

        public int ShopCartId { get; set; }

        [ForeignKey(nameof(ShopCartId))]
        [ValidateNever]
        public ShopCart? ShopCart { get; set; }

        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        [ValidateNever]
        public Product? Product { get; set; }

        [Range(1, 1000, ErrorMessage = "Must be a value between 1 and 1000")]
        public int Quantity { get; set; }

        [NotMapped]
        [Range(0.01, double.MaxValue)]
        public double TotalPriceItem { get; set; }
    }
}