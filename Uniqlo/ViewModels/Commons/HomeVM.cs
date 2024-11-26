﻿using Uniqlo.ViewModels.Products;
using Uniqlo.ViewModels.Sliders;

namespace Uniqlo.ViewModels.Commons;

public class HomeVM
{
    public IEnumerable<SliderListItemVM> Sliders { get; set; }
    public IEnumerable<ProductListItemVM> Products { get; set; }
}
