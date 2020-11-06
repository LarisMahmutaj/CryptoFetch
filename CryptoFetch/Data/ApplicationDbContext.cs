using Microsoft.EntityFrameworkCore;
using CryptoFetch.Models;

namespace CryptoFetch.Data
{
    public class ApplicationDbContext : DbContext{
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Favourite> Favourites { get; set; }
    }
}
