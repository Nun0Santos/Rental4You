using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rental4You.Models;

namespace Rental4You.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Car> cars { get; set; }
        public DbSet<Delivery> deliveries { get; set; }
        public DbSet<Reservation> reservations { get; set; }
        public DbSet<Returnal> returnals { get; set; }

    }
}