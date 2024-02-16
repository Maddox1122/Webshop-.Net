using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Webshop_089667.Pages
{
    public class UpdateModel : PageModel
    {
        private readonly SqliteConnection connection;
        private readonly IWebHostEnvironment _environment;

        [BindProperty]
        public Product Product { get; set; }

        [BindProperty]
        public IFormFile Upload { get; set; }

        public UpdateModel(IWebHostEnvironment environment)
        {
            _environment = environment;
            SqliteConnectionStringBuilder connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "motoren.db";
            connection = new SqliteConnection(connectionStringBuilder.ToString());
        }

        public IActionResult OnGet(int artikelnummer)
        {
            if (HttpContext.Session.GetString("AdminIngelogd") != "true")
            {
                return RedirectToPage("/Index");
            }
            else
            {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM producten WHERE ID = {artikelnummer}";
                SqliteDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    Product = new Product
                    {
                        Artikelnummer = reader.GetInt32(0),
                        Naam = reader.GetString(1),
                        Prijs = (decimal)reader.GetDouble(2),
                        Afbeelding = reader.GetString(3)
                    };
                }

                connection.Close();

                if (Product == null)
                {
                    return NotFound();
                }

                return Page();
            }
        }

        public IActionResult OnPost()
        {
            if (Upload != null)
            {
                if (IsImageValid(Upload))
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Upload.FileName);
                    var filePath = Path.Combine(_environment.WebRootPath, "Images", fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        Upload.CopyTo(fileStream);
                    }

                    if (ModelState.IsValid)
                    {
                        connection.Open();
                        using (SqliteCommand command = connection.CreateCommand())
                        {
                            command.CommandText = "UPDATE producten SET Naam = @Naam, Prijs = @Prijs, Afbeelding = @Afbeelding WHERE ID = @Artikelnummer";
                            command.Parameters.AddWithValue("@Naam", Product.Naam);
                            command.Parameters.AddWithValue("@Prijs", Product.Prijs);
                            command.Parameters.AddWithValue("@Afbeelding", fileName);
                            command.Parameters.AddWithValue("@Artikelnummer", Product.Artikelnummer);

                            command.ExecuteNonQuery();
                        }
                        connection.Close();

                        return RedirectToPage("/Beheer");
                    }
                }
                else
                {
                    ModelState.AddModelError("Upload", "Bestand is geen afbeelding");
                }
            }
            return Page();
        }

        private bool IsImageValid(IFormFile file)
        {
            return file.ContentType == "image/gif" || file.ContentType == "image/jpeg" || file.ContentType == "image/png" || file.ContentType == "image/jpg";
        }
    }
}
