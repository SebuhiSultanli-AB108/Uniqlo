using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uniqlo.Context;
using Uniqlo.ViewModels.Basket;
using Uniqlo.ViewModels.Brands;
using Uniqlo.ViewModels.Products;
using Uniqlo.ViewModels.Shops;
using System.Text.Json;
using System.Security.Claims;
using Uniqlo.Models;
using Microsoft.AspNetCore.Identity;


namespace Uniqlo.Controllers;

public class ShopController(UniqloDbContext _context, UserManager<User> _userManager) : Controller
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
    public IActionResult CommentPage()
    {
        return View();
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
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> RemoveBasket(int id)
    {
        var basket = getBasket();
        basket.Remove(basket.FirstOrDefault(x => x.Id == id));
        string data = JsonSerializer.Serialize(basket);
        HttpContext.Response.Cookies.Append("basket", data);
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> GetBasket()
    {
        return Json(getBasket());
    }
    public async Task<IActionResult> AddComment(int id, string description)
    {
        if (!ModelState.IsValid) return View();
        User user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
        CommentItem comment = new CommentItem
        {
            UserName = user.UserName,
            UserId = user.Id,
            Description = description,
            CreatedTime = DateTime.Now,
            ProductId = _context.Products.FirstOrDefault(x => x.Id == id).Id,
        };
        await _context.CommentItems.AddAsync(comment);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> RemoveComment(int id)
    {
        CommentItem target = await _context.CommentItems.FindAsync(id);
        if (!target.IsDeleted)
        {
            target.IsDeleted = true;
            _context.CommentItems.Update(target);
        }
        else
        {
            _context.CommentItems.Remove(target);
        }
        await _context.SaveChangesAsync();
        return RedirectToAction("");
    }
    public async Task<IActionResult> Details(int? id)
    {
        if (!id.HasValue) return BadRequest();
        var data = await _context.Products
            .Include(x => x.Images)
            .Include(x => x.ProductRating)
            .Where(x => x.Id == id.Value && !x.IsDeleted).FirstOrDefaultAsync();
        if (data is null) return NotFound();
        string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId is not null)
        {
            var rating = await _context.productRatings.Where(x => x.UserId == userId && x.ProductId == id).Select(x => x.RatingRate).FirstOrDefaultAsync();
            ViewBag.Rating = rating == 0 ? 5 : rating;
        }
        else
            ViewBag.Rating = 5;
        DeteailsVM vm = new DeteailsVM();
        vm.Product = data;
        vm.Comments = await _context.CommentItems.ToListAsync();
        return View(vm);
    }
    List<CookieItemVM> getBasket()
    {
        string? value = HttpContext.Request.Cookies["basket"];
        if (value == null) return new();
        return JsonSerializer.Deserialize<List<CookieItemVM>>(value) ?? new();
    }
}
