using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uniqlo.Context;
using Uniqlo.ViewModels.Commons;
using Uniqlo.ViewModels.Products;
using Uniqlo.ViewModels.Sliders;

namespace Uniqlo.Controllers
{
    public class HomeController(UniqloDbContest _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            HomeVM vm = new();
            vm.Sliders = await _context.Sliders.Select(s => new SliderListItemVM
            {
                ImageUrl = s.ImageUrl,
                Link = s.Link,
                Subtitle = s.Title,
                Title = s.Title,
            }).ToListAsync();
            vm.Products = await _context.Products.Select(p => new ProductListItemVM
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
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult Shop()
        {
            return View();
        }
    }
}
