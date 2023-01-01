using Microsoft.AspNetCore.Identity;

namespace Rental4You.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime BirthDate { get; set; }
        public int TaxNumber { get; set; }
        //public int CompanyId { get; set; }
        //public ICollection<Reservation> Reservations { get; set; }
    }
}
