using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Uniqlo.Models;

namespace Uniqlo.Context;

public class UniqloDbContext : IdentityDbContext<User>
{
    public DbSet<Slider> Sliders { get; set; }
    public DbSet<CommentItem> CommentItems { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductRating> productRatings { get; set; }
    public UniqloDbContext(DbContextOptions opt) : base(opt) { }
}
