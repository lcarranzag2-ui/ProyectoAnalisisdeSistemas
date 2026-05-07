using Microsoft.EntityFrameworkCore;
namespace HiddenValley.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Models.TipoCabana> TipoCabanas { get; set; }
}
}