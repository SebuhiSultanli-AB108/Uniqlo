using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uniqlo.Context;
using Uniqlo.Models;
using Uniqlo.ViewModels.Brands;

namespace Uniqlo.Areas.Admin.Controllers;

[Area("Admin")]
public class BrandController(UniqloDbContest _context) : Controller
{
    public async Task<IActionResult> Index() { return View(await _context.Brands.ToListAsync()); }
    public IActionResult Create() { return View(); }
    public IActionResult Update() { return View(); }

    [HttpPost]
    public async Task<IActionResult> Create(BrandAndProductVM vm)
    {
        if (!ModelState.IsValid) return View(vm);
        Brand brand = new Brand
        {
            Name = vm.Name,
            CreatedTime = DateTime.Now,
        };
        await _context.Brands.AddAsync(brand);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        Brand target = await _context.Brands.FindAsync(id);
        if (!target.IsDeleted)
        {
            target.IsDeleted = true;
            _context.Brands.Update(target);
        }
        else
        {
            _context.Brands.Remove(target);
        }
        await _context.SaveChangesAsync();
        return RedirectToAction("");
    }
    [HttpPost]
    public async Task<IActionResult> Update(BrandAndProductVM vm, int id)
    {
        if (!ModelState.IsValid) return View(vm);
        Brand brand = await _context.Brands.FindAsync(id);
        brand.Name = vm.Name;
        _context.Brands.Update(brand);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
