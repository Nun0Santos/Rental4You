using Rental4You.Models;
using System.ComponentModel.DataAnnotations;

namespace Rental4You.ViewModels
{
    public class ReservationViewModel
    {
        [Display(Name = "PickupDate", Prompt = "yyyy-mm-dd")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [DataType(DataType.DateTime)]
        public DateTime start { get; set; }
        [Display(Name = "ReturnDate", Prompt = "yyyy-mm-dd")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [DataType(DataType.DateTime)]
        public DateTime end { get; set; }
        [Display(Name = "Car", Prompt = "Id")]
        public int carID { get; set; }
        public string carMaker { get; set; }
        public string carModel { get; set; }
    }
}
