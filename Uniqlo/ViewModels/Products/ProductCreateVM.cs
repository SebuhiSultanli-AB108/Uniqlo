using System.ComponentModel.DataAnnotations;
using Uniqlo.Models;

namespace Uniqlo.ViewModels.Products;

public class ProductCreateVM
{
    [MaxLength(32)]
    public string Name { get; set; }
    [MaxLength(512)]
    public string Description { get; set; }
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }
    [DataType("decimal(18,2)")]
    public decimal BuyPrice { get; set; }
    [DataType("decimal(18,2)")]
    public decimal SellPrice { get; set; }
    [Range(0, 100)]
    public int Discount { get; set; }
    public int? BrandId { get; set; }
    public IFormFile File { get; set; }
    public ICollection<IFormFile> OtherFiles { get; set; }
    public static implicit operator Product(ProductCreateVM vm)
    {
        return new Product
        {
            BrandId = vm.BrandId,
            Name = vm.Name,
            Description = vm.Description,
            Quantity = vm.Quantity,
            BuyPrice = vm.BuyPrice,
            SellPrice = vm.SellPrice,
            Discount = vm.Discount
        };
    }
}
