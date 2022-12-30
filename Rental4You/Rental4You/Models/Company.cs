namespace Rental4You.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int  Rating { get; set; } // 0-5
        public List<ApplicationUser> Employees { get; set; } //todos os funcionarios desta empresa
    }
}
