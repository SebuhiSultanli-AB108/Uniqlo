using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uniqlo.Context;
using Uniqlo.Helpers;
using Uniqlo.ViewModels.Basket;

namespace Uniqlo.ViewComponents
{
    public class LayoutHeaderViewComponent(UniqloDbContext _context) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var basket = BasketHelper.GetBasket(HttpContext.Request);
            var basketItems = await _context.Products
                .Where(y => basket.Select(x => x.Id)
                .Contains(y.Id))
                .Select(x => new BasketItemVM
                {
                    Id = x.Id,
                    Name = x.Name,
                    ImageUrl = x.CoverImage,
                    SellPrice = x.SellPrice,
                    Discount = x.Discount
                })
                .ToListAsync();
            foreach (var item in basketItems)
            {
                item.Count = basket.First(x => x.Id == item.Id).Count;
            }
            return View(basketItems);
        }
    }
}
