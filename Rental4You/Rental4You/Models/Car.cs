using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Rental4You.Models
{
    public class Car
    {
        public int Id { get; set; }
        [Display(Name = "Marker", Prompt = "Enter the car brand")]

        public string Maker { get; set; }
        [Display(Name = "Model", Prompt = "Enter the car model")]

        public string Model { get; set; }
        public string Type { get; set; }

        public string Transmission { get; set; } //Automatic ou Manual

        public int Seats { get; set; }
        public int Year { get; set; }
        public string LicensePlate { get; set; }
        [Display(Name = "Location", Prompt = "Enter car location")]

        public string Location { get; set; }
        public double Km { get; set; }
        public string state { get; set; }
    }
}
