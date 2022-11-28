namespace Rental4You.Models
{
    public class Delivery
    {
        public int Id { get; set; }
        public double Km { get; set; }
        public string State { get; set; }
        public bool Damage { get; set; }
        public string Observations { get; set; }
    }
}
