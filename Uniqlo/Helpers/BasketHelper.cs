using System.Text.Json;
using Uniqlo.ViewModels.Basket;

namespace Uniqlo.Helpers;

public class BasketHelper
{
    public static List<CookieItemVM> GetBasket(HttpRequest request)
    {

        string? value = request.Cookies["basket"];
        if (value is null) return new();
        return JsonSerializer.Deserialize<List<CookieItemVM>>(value) ?? new();
    }
}
