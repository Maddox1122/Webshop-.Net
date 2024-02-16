using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using System.ComponentModel.DataAnnotations;

namespace Webshop_089667.Pages
{
    public class BestelModel : PageModel
    {
        SqliteConnection connection;
        public Product SelectedProduct { get; set; }
        public int ArtikelNummer { get; set; }
        public string ArtikelNaam { get; set; }
        public decimal ArtikelPrijs { get; set; }
        public string ArtikelAfbeelding { get; set; }
        [BindProperty(SupportsGet = true)]
        public int artikelnummer { get; set; }
        [BindProperty, Required(ErrorMessage = "Voer uw naam in"), Display(Name = "Naam:"), MaxLength(10, ErrorMessage = "De naam mag niet langer zijn dan 10 karakters.")]
        public string naam { get; set; }

        [BindProperty, Required(ErrorMessage = "Voer uw adres in"), Display(Name = "Adres:")]
        public string adres { get; set; }
        [BindProperty, Required(ErrorMessage = "Voer uw woonplaats in"), Display(Name = "Woonplaats:")]
        public string woonplaats { get; set; }


        public BestelModel()
        {
            SqliteConnectionStringBuilder connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "motoren.db";
            connection = new SqliteConnection(connectionStringBuilder.ToString());
        }

        public IActionResult OnGet()
        {
            if (artikelnummer != null)
            {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM producten where id = {artikelnummer}";
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ArtikelNummer = reader.GetInt32(0);
                    ArtikelNaam = reader.GetString(1);
                    ArtikelPrijs = (decimal)reader.GetDouble(2);
                    ArtikelAfbeelding = reader.GetString(3);

                    artikelnummer = ArtikelNummer;
                }
                connection.Close();
            }
            else
            {
                return RedirectToPage("Error");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                connection.Open();
                SqliteCommand insertCommand = connection.CreateCommand();
                insertCommand.CommandText = "INSERT INTO bestellingen (naam, adres, woonplaats, bestelling) VALUES (@naam, @adres, @woonplaats, @bestelling)";
                insertCommand.Parameters.AddWithValue("@naam", naam);
                insertCommand.Parameters.AddWithValue("@adres", adres);
                insertCommand.Parameters.AddWithValue("@woonplaats", woonplaats);
                insertCommand.Parameters.AddWithValue("@bestelling", artikelnummer);
                int rowsAffected = insertCommand.ExecuteNonQuery();
                connection.Close();

                if (rowsAffected > 0)
                {
                    return RedirectToPage("Index");
                }
                else
                {
                    return Page();
                }
            }
            return OnGet();
        }
    }
}
