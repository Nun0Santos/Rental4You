namespace Rental4You.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Car car { get; set; }
        public int carId { get; set; } //FK
        public Delivery delivery { get; set; }
        public int deliveryId { get; set; } //fk
        public Returnal returnal { get; set; }
        public int returnalId { get; set; } //FK
        public float price { get; set; }
    }
}
