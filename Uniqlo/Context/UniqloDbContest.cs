using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Uniqlo.Models;

namespace Uniqlo.Context;

public class UniqloDbContest : IdentityDbContext<User>
{
    public DbSet<Slider> Sliders { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Product> Products { get; set; }
    public UniqloDbContest(DbContextOptions opt) : base(opt) { }
}
