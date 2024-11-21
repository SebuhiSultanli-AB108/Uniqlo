using Microsoft.EntityFrameworkCore;
using Uniqlo.Models;

namespace Uniqlo.Context
{
    public class UniqloDbContest : DbContext
    {
        public DbSet<Slider> Sliders { get; set; }
        public UniqloDbContest(DbContextOptions opt) : base(opt) { }
    }
}
