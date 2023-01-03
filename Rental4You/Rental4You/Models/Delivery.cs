namespace Rental4You.Models
{
    public class Delivery
    {
        public int Id { get; set; }
        public double Km { get; set; }
        public bool Damage { get; set; }
        public string Observations { get; set; }
        public Car Car { get; set; }
        public int CarId { get; set; }
        public ApplicationUser Employee { get; set; }
        public string? EmployeeId { get; set; }
        public DateTime DateOfDelivery { get; set; }
        public Reservation Reservation { get; set; }
        public int ReservationId { get; set; }
        public bool isReceived { get; set; }

    }
}
