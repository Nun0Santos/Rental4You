using System.ComponentModel.DataAnnotations;

namespace Rental4You.Models
{
    public class Company
    {
        public int Id { get; set; }
        [Display(Name = "Company")]
        public string Name { get; set; }
        public int Rating { get; set; }

        public List<ApplicationUser> Employees { get; set; } //todos os funcionarios desta empresa
    }
}
