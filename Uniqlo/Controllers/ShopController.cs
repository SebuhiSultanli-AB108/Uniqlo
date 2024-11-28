using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uniqlo.Context;
using Uniqlo.ViewModels.Basket;
using Uniqlo.ViewModels.Brands;
using Uniqlo.ViewModels.Products;
using Uniqlo.ViewModels.Shops;
using System.Text.Json;


namespace Uniqlo.Controllers;

public class ShopController(UniqloDbContest _context) : Controller
{
    public async Task<IActionResult> Index(int? brandId, string amount)
    {
        var query = _context.Products.AsQueryable();
        if (brandId.HasValue)
        {
            query = query.Where(x => x.BrandId == brandId);
        }
        if (amount != null)
        {
            var prices = amount.Split('-').Select(x => Convert.ToInt32(x));
            query = query.Where(y => prices.ElementAt(0) <= y.SellPrice && prices.ElementAt(1) >= y.SellPrice);
        }
        ShopVM vm = new ShopVM();
        vm.Brands = await _context.Brands
            .Where(x => !x.IsDeleted)
            .Select(x => new BrandAndProductVM
            {
                Id = x.Id,
                Name = x.Name,
                Count = x.Products.Count
            }).ToListAsync();
        vm.Products = await query
            .Take(6)
            .Select(p => new ProductListItemVM
            {
                CoverImage = p.CoverImage,
                Discount = p.Discount,
                Id = p.Id,
                IsInStock = p.Quantity > 0,
                Name = p.Name,
                SellPrice = p.SellPrice,
            }).ToListAsync();
        return View(vm);
    }

    public async Task<IActionResult> AddBasket(int id)
    {
        var basket = getBasket();
        var item = basket.FirstOrDefault(x => x.Id == id);
        if (item != null)
            item.Count++;
        else
            basket.Add(new CookieItemVM { Id = id, Count = 1 });

        string data = JsonSerializer.Serialize(basket);
        HttpContext.Response.Cookies.Append("basket", data);
        return Ok();
    }
    public async Task<IActionResult> GetBasket()
    {
        return Json(getBasket());
    }
    List<CookieItemVM> getBasket()
    {
        string? value = HttpContext.Request.Cookies["basket"];
        if (value == null) return new();
        return JsonSerializer.Deserialize<List<CookieItemVM>>(value) ?? new();
    }
}
