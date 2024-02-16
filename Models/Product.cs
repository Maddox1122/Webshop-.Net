using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

public class Product
{
    public int Artikelnummer { get; set; }
    [Required(ErrorMessage = "Voer de naam van het fietsshirt in.")]
    [Display(Name = "Naam van het fietsshirt:")]
    [MaxLength(50, ErrorMessage = "De naam mag niet langer zijn dan 50 karakters.")]
    public string Naam { get; set; }
    [Required(ErrorMessage = "Voer de prijs van het fietsshirt in.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "De prijs moet groter zijn dan 0.")]
    public decimal? Prijs { get; set; }
    public string? Afbeelding { get; set; }
}
