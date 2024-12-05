using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Uniqlo.Models;
using Uniqlo.Views.Account.Enums;

namespace Uniqlo.Extensions;

public static class SeedExtension
{
    public static void UseUserSeed(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            CreateRoles(roleManager).Wait();
            CreateUsers(userManager).Wait();
        }
    }
    private static async Task CreateRoles(RoleManager<IdentityRole> _roleManager)
    {
        if (!await _roleManager.Roles.AnyAsync())
            foreach (Roles role in Enum.GetValues(typeof(Roles)))
            {
                await _roleManager.CreateAsync(new IdentityRole(role.GetRole()));
            }
    }
    private static async Task CreateUsers(UserManager<User> _userManager)
    {
        if (!await _userManager.Users.AnyAsync(u => u.NormalizedUserName == "ADMIN"))
        {
            User user = new User();
            user.UserName = "admin";
            user.Fullname = "admin";
            user.Email = "admin@gmail.com";
            string role = nameof(Roles.Admin);
            await _userManager.CreateAsync(user, "123");
            await _userManager.AddToRoleAsync(user, role);
        }
    }
}