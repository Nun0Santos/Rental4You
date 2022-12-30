using Rental4You.Models;

namespace Rental4You.ViewModels
{
    public class ReservationViewModel
    {
        public string Client { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public Car car { get; set; }
    }
}
