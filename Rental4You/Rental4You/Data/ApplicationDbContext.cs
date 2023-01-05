using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rental4You.Models;

namespace Rental4You.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Car> cars { get; set; }
        public DbSet<Delivery> deliveries { get; set; }
        public DbSet<Reservation> reservations { get; set; }
        public DbSet<Rental4You.Models.Company> Company { get; set; }
        public DbSet<Category> categories { get; set; }

    }
}