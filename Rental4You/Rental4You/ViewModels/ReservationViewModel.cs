using Rental4You.Models;
using System.ComponentModel.DataAnnotations;

namespace Rental4You.ViewModels
{
    public class ReservationViewModel
    {
        [Display(Name = "Start Date", Prompt = "yyyy-mm-dd")]
        public DateTime start { get; set; }
        [Display(Name = "End Date", Prompt = "yyyy-mm-dd")]
        public DateTime end { get; set; }
        [Display(Name = "Car", Prompt = "Id")]
        public int carID { get; set; }
        public string carMaker { get; set; }
        public string carModel { get; set; }
    }
}
