using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uniqlo.Context;
using Uniqlo.Extensions;
using Uniqlo.Models;
using Uniqlo.ViewModels.Sliders;
using Uniqlo.Views.Account.Enums;

namespace Uniqlo.Areas.Admin.Controllers;

[Area("Admin"), Authorize(Roles = nameof(Roles.Admin))]
public class SliderController(UniqloDbContext _context, IWebHostEnvironment _env) : Controller
{
    public async Task<IActionResult> Index() { return View(await _context.Sliders.ToListAsync()); }
    public IActionResult Create() { return View(); }
    public IActionResult Update() { return View(); }

    [HttpPost]
    public async Task<IActionResult> Create(SliderCreateVM vm)
    {
        if (!ModelState.IsValid) return View(vm);
        if (!vm.File.ContentType.StartsWith("image"))
        {
            ModelState.AddModelError("File", "Format type must be an image.");
            return View(vm);
        }
        if (vm.File.Length > 2 * 1024 * 1024)
        {
            ModelState.AddModelError("File", "File size must be less than 2 MB.");
            return View(vm);
        }
        string newName = Path.GetRandomFileName() + Path.GetExtension(vm.File.FileName);
        using (Stream stream = System.IO.File.Create(Path.Combine(_env.WebRootPath, "imgs", "sliders", newName)))
        {
            await vm.File.CopyToAsync(stream);
        }
        Slider slider = new Slider
        {
            ImageUrl = newName,
            Title = vm.Title,
            Subtitle = vm.Subtitle!,
            Link = vm.Link
        };
        await _context.Sliders.AddAsync(slider);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Delete(int id)
    {
        Slider target = await _context.Sliders.FindAsync(id);
        if (!target.IsDeleted)
        {
            target.IsDeleted = true;
            _context.Sliders.Update(target);
        }
        else
        {
            _context.Sliders.Remove(target);
        }
        await _context.SaveChangesAsync();
        return RedirectToAction("");
    }
    [HttpPost]
    public async Task<IActionResult> Update(SliderCreateVM vm, int id)
    {
        if (!ModelState.IsValid) return View(vm);
        Slider slider = await _context.Sliders.FindAsync(id);
        slider.Title = vm.Title;
        slider.Subtitle = vm.Subtitle;
        slider.Link = vm.Link;
        slider.ImageUrl = await vm.File!.UploadAsync(_env.WebRootPath, "imgs", "sliders");
        _context.Sliders.Update(slider);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}