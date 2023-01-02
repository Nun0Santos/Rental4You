using System.ComponentModel.DataAnnotations;

namespace Rental4You.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        [Display(Name = "Pickup Date", Prompt = "yyyy-mm-dd")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Start { get; set; }
        [Display(Name = "Return Date", Prompt = "yyyy-mm-dd")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime End { get; set; }
        public Car Car { get; set; }
        public int CarId { get; set; }
        public ApplicationUser Client { get; set; }
        public string ClientId { get; set; }
        public decimal Price { get; set; }
        public bool Confirmed { get; set; }
    }
}
