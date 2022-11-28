using Rental4You.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rental4You.ViewModels
{
    public class SearchCarViewModel
    {
        public List<Car> ListOfCars { get; set; }

        [Display(Name = "PickupDate", Prompt = "yyyy-mm-dd")]
        public DateTime PickupDate {get;set;}

        [Display(Name = "ReturnDate", Prompt = "yyyy-mm-dd")]
        public DateTime ReturnDate { get; set; }
        public string TextToSearch { get; set; }
        public int NumberOfResults { get; set; }



    }
}
