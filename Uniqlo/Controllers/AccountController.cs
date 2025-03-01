﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Uniqlo.Models;
using Uniqlo.Service.Abstract;
using Uniqlo.ViewModels.Auths;
using Uniqlo.Views.Account.Enums;

namespace Uniqlo.Controllers
{
    public class AccountController(
        UserManager<User> _userManager,
        SignInManager<User> _signInManager,
        RoleManager<IdentityRole> _roleManager,
        IEmailService _emailService) : Controller
    {
        private bool isAuthenticated => HttpContext.User.Identity?.IsAuthenticated ?? false;
        public IActionResult Register()
        {
            if (isAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }
        public IActionResult Login()
        {
            if (isAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }
        public IActionResult UpdatePassword()
        {
            if (!isAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(ResetPasswordVM vm)
        {
            if (!isAuthenticated) return RedirectToAction("Index", "Home");
            if (!ModelState.IsValid) return View();
            User user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            user.EmailConfirmed = true;
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            _emailService.SendEmailConfirmation(user.Email, user.UserName, token);
            await _userManager.ResetPasswordAsync(user, token, vm.Password);
            return Content("Please verify the email address!");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (isAuthenticated) return RedirectToAction("Index", "Home");
            if (!ModelState.IsValid) return View();
            User user = new User
            {
                Fullname = vm.Username,
                Email = vm.Email,
                UserName = vm.Username
            };
            var result = await _userManager.CreateAsync(user, vm.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }
            var roleResult = await _userManager.AddToRoleAsync(user, nameof(Roles.User));
            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            _emailService.SendEmailConfirmation(user.Email, user.UserName, token);
            return Content("Please verify the email address!");
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm, string? returnUrl = null)
        {
            if (isAuthenticated) return RedirectToAction("Index", "Home");
            if (!ModelState.IsValid) return View();
            User? user = null;
            if (vm.UsernameOrEmail.Contains('@'))
                user = await _userManager.FindByEmailAsync(vm.UsernameOrEmail);
            else
                user = await _userManager.FindByNameAsync(vm.UsernameOrEmail);
            if (user is null)
            {
                ModelState.AddModelError("", "Username or password is wrong!");
                return View();
            }
            var result = await _signInManager.PasswordSignInAsync(user, vm.Password, vm.RememberMe, true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                    ModelState.AddModelError("", "Wait until" + user.LockoutEnd!.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                if (result.IsNotAllowed)
                    ModelState.AddModelError("", "Confirm your account!");
                return View();
            }
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                    return RedirectToAction("Index", new { Controller = "Dashboard", Area = "Admin" });
                return RedirectToAction("Index", "Home");
            }
            return LocalRedirect(returnUrl);
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
        public async Task<IActionResult> VerifyEmail(string token, string user)
        {
            var entity = await _userManager.FindByNameAsync(user);
            if (entity is null) return BadRequest();
            var result = await _userManager.ConfirmEmailAsync(entity, token.Replace(' ', '+'));
            if (!result.Succeeded)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var item in result.Errors)
                {
                    stringBuilder.AppendLine(item.Description);
                }
                return Content(stringBuilder.ToString());
            }
            await _signInManager.SignInAsync(entity, true);
            return RedirectToAction("Index", "Home");
        }
    }
}