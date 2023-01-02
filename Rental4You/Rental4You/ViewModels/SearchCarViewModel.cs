using Rental4You.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rental4You.ViewModels
{
    public class SearchCarViewModel
    {
        public List<Car> ListOfCars { get; set; }

        [Display(Name = "PickupDate", Prompt = "yyyy-mm-dd")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.DateTime)]
        public DateTime PickupDate {get;set;}

        [Display(Name = "ReturnDate", Prompt = "yyyy-mm-dd")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.DateTime)]
        public DateTime ReturnDate { get; set; }

        [Display(Name = "PickupLocation", Prompt = "City")]
        public string PickupLocation { get; set; }

        [Display(Name = "VehicleType", Prompt = "SUV,Sport")]
        public string VehicleType { get; set; }
        public string TextToSearch { get; set; }
        public int NumberOfResults { get; set; }

    }
}
