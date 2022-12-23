namespace Rental4You.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<ApplicationUser> Employees { get; set; } //todos os funcionarios desta empresa
    }
}
