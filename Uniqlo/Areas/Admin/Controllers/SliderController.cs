﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uniqlo.Context;
using Uniqlo.Models;
using Uniqlo.ViewModels.Sliders;

namespace Uniqlo.Areas.Admin.Controllers;

[Area("Admin")]
public class SliderController(UniqloDbContest _context, IWebHostEnvironment _env) : Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await _context.Sliders.ToListAsync());
    }
    public IActionResult Create()
    {
        return View();
    }
    public IActionResult Update()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(SliderCreateVM vm)
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
}
/*
Admin paneldə Slider table-ında Update, Delete, Hide (Show) actionları yazdıq.
Update slider məlumatlarını yeniləmək üçün, Delete slideri databazadan, sliderin şəkilini isə qovluqdan silmək üçündür, Hide isə slider-in IsDeleted sütununu dəyişdirir.
Əgər IsDeleted true-dırsa şəkili qovluqdan və ya databazadan silmirsiz sadəcə olaraq istifadəçi həmin slide-ı görmür.
Admin yenidən Show  düyməsinə kliklədiyi zaman artıq istifadəçiyə görünür olur.
*/
