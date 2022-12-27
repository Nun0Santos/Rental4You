using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rental4You.Models
{
    public class Car
    {
        public int Id { get; set; }
        [Display(Name = "Maker", Prompt = "Enter the car brand")]

        public string Maker { get; set; }
        [Display(Name = "Model", Prompt = "Enter the car model")]

        public string Model { get; set; }
        [Display(Name = "Type", Prompt = "Enter the car type")]

        public string Type { get; set; }
        [Display(Name = "Transmission", Prompt = "Enter the car transmission")]

        public string Transmission { get; set; } //Automatic ou Manual
        [Display(Name = "Seats", Prompt = "Enter the car seats")]
        public int Seats { get; set; }
        [Display(Name = "Year", Prompt = "Enter the car year")]
        public int Year { get; set; }
        [Display(Name = "LicensePlate", Prompt = "Enter the car license plate")]
        public string LicensePlate { get; set; }
        [Display(Name = "Location", Prompt = "Enter the car location")]
        public string Location { get; set; }
        [Display(Name = "Km", Prompt = "Enter the car kilometers")]
        public double Km { get; set; }
        [Display(Name = "State", Prompt = "Enter the state of the car")]
        public string state { get; set; }
        [Display(Name = "Price", Prompt = "Enter the price of the car")]
        public float price { get; set; }
        [Display(Name = "Fuel", Prompt = "Enter the type of fuel of the car")]
        public string fuel { get; set; }
        public Company Company { get; set; } 
        public int CompanyId { get; set; }

    }
}
