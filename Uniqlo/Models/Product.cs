﻿using System.ComponentModel.DataAnnotations;

namespace Uniqlo.Models;

public class Product : BaseEntity
{
    [MaxLength(32)]
    public string Name { get; set; }
    [MaxLength(512)]
    public string Description { get; set; }
    public string CoverImage { get; set; }
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }
    [DataType("decimal(18,2)")]
    public decimal BuyPrice { get; set; }
    [DataType("decimal(18,2)")]
    public decimal SellPrice { get; set; }
    [Range(0, 100)]
    public int Discount { get; set; }
    public int? BrandId { get; set; }
    public Brand Brand { get; set; }
    public ICollection<ProductImage>? Images { get; set; }
    public ICollection<ProductRating>? ProductRating { get; set; }
}