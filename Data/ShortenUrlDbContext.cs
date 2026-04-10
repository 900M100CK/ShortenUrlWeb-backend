using Microsoft.EntityFrameworkCore;
using ShortenUrlWeb.Models;

namespace ShortenUrlWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet này sẽ đại diện cho bảng 'ShortenUrls' trong Database của bạn
        public DbSet<ShortenUrlModel> ShortenUrls { get; set; }
    }
}