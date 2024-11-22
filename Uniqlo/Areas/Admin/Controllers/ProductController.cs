using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uniqlo.Context;
using Uniqlo.Extensions;
using Uniqlo.Models;
using Uniqlo.ViewModels.Products;

namespace Uniqlo.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController(IWebHostEnvironment _env, UniqloDbContest _context) : Controller
{
    public async Task<IActionResult> IndexAsync()
    {
        return View(await _context.Products
            .Include(p => p.Brand)
            .ToListAsync());
    }
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _context.Brands.Where(x => !x.IsDeleted).ToListAsync();
        return View();
    }

    public async Task<IActionResult> Update()
    {
        ViewBag.Categories = await _context.Brands.Where(x => !x.IsDeleted).ToListAsync();
        return View();
    }


    public async Task<IActionResult> Delete(int id)
    {
        Product target = await _context.Products.FindAsync(id);
        if (!target.IsDeleted)
        {
            target.IsDeleted = true;
            _context.Products.Update(target);
        }
        else
        {
            _context.Products.Remove(target);
        }
        await _context.SaveChangesAsync();
        return RedirectToAction("");
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateVM vm)
    {
        if (vm.File != null)
        {
            int fileMaxSize = 400;

            if (!vm.File.IsValidType("image"))
                ModelState.AddModelError("File", "File must be and image.");
            if (!vm.File.IsValidSize(fileMaxSize))
                ModelState.AddModelError("File", $"File must be smaller than {fileMaxSize}KB.");
        }
        if (!ModelState.IsValid) return View(vm);
        if (!await _context.Brands.AnyAsync(x => x.Id == vm.BrandId))
        {
            ModelState.AddModelError("BrandId", "Brand not found.");
            return View();
        }
        Product product = vm;
        product.CoverImage = await vm.File!.UploadAsync(_env.WebRootPath, "imgs", "products");
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Create));
    }
    [HttpPost]
    public async Task<IActionResult> Update(ProductCreateVM vm, int id)

    {
        if (vm.File != null)
        {
            int fileMaxSize = 400;

            if (!vm.File.IsValidType("image"))
                ModelState.AddModelError("File", "File must be and image.");
            if (!vm.File.IsValidSize(fileMaxSize))
                ModelState.AddModelError("File", $"File must be smaller than {fileMaxSize}KB.");
        }
        if (!ModelState.IsValid) return View(vm);
        if (!await _context.Brands.AnyAsync(x => x.Id == vm.BrandId))
        {
            ModelState.AddModelError("BrandId", "Brand not found.");
            return View();
        }
        Product product = await _context.Products.FindAsync(id);
        product.CoverImage = await vm.File!.UploadAsync(_env.WebRootPath, "imgs", "products");
        product.Name = vm.Name;
        product.Description = vm.Description;
        product.Quantity = vm.Quantity;
        product.BuyPrice = vm.BuyPrice;
        product.SellPrice = vm.SellPrice;
        product.Discount = vm.Discount;
        product.BrandId = vm.BrandId;

        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Create));
    }
}