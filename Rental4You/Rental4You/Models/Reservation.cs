namespace Rental4You.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Car Car { get; set; }
        public int CarId { get; set; }
        public ApplicationUser Client { get; set; }
        public string ClientId { get; set; }
        public decimal Price { get; set; }
        public bool Confirmed { get; set; }
    }
}
