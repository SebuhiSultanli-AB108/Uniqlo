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
    int fileMaxSize = 400;

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
            if (!vm.File.IsValidType("image"))
                ModelState.AddModelError("File", "File must be and image.");
            if (!vm.File.IsValidSize(fileMaxSize))
                ModelState.AddModelError("File", $"File must be smaller than {fileMaxSize}KB.");
        }
        if (vm.OtherFiles.Any())
        {
            if (!vm.OtherFiles.All(x => x.IsValidType("image")))
                ModelState.AddModelError("OtherFiles", "One ore more images are not in image type!");
            if (!vm.OtherFiles.All(x => x.IsValidSize(fileMaxSize)))
                ModelState.AddModelError("OtherFiles", $"Files must be smaller than {fileMaxSize}KB.");
        }
        if (!ModelState.IsValid) return View(vm);
        if (!await _context.Brands.AnyAsync(x => x.Id == vm.BrandId))
        {
            ModelState.AddModelError("BrandId", "Brand not found.");
            return View();
        }
        Product product = vm;
        product.CoverImage = await vm.File!.UploadAsync(_env.WebRootPath, "imgs", "products");
        product.Images = vm.OtherFiles.Select(x => new ProductImage
        {
            ImageUrl = x.UploadAsync(_env.WebRootPath, "imgs", "products").Result
        }).ToList();
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(IndexAsync));
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
        if (vm.OtherFiles.Any())
        {
            if (!vm.OtherFiles.All(x => x.IsValidType("image")))
            {
                string fileNames =
                    string.Join(',', vm.OtherFiles
                    .Where(x => x.IsValidType("image"))
                    .Select(x => x.FileName));
                ModelState.AddModelError("OtherFiles", fileNames + " is not an image");
            }
            if (!vm.OtherFiles.All(x => x.IsValidSize(fileMaxSize)))
            {
                string fileNames =
                   string.Join(',', vm.OtherFiles
                   .Where(x => x.IsValidSize(fileMaxSize))
                   .Select(x => x.FileName));
            }
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
        product.Images = vm.OtherFiles
            .Select(x => new ProductImage
            {
                ImageUrl = x.UploadAsync(_env.WebRootPath, "imgs", "products").Result
            }).ToList();
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Create));
    }
}