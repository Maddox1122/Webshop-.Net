using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Webshop_089667.Pages
{
    public class InsertModel : PageModel
    {
        private readonly SqliteConnection connection;
        private readonly IWebHostEnvironment _environment;

        [BindProperty]
        public Product Product { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Selecteer een afbeelding.")]
        public IFormFile? Upload { get; set; }

        public InsertModel(IWebHostEnvironment environment)
        {
            _environment = environment;
            SqliteConnectionStringBuilder connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "motoren.db";
            connection = new SqliteConnection(connectionStringBuilder.ToString());
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("AdminIngelogd") != "true")
            {
                return RedirectToPage("/Index");
            }
            else
            {
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
                        SqliteCommand command = connection.CreateCommand();
                        command.CommandText = "INSERT INTO producten (Naam, Prijs, Afbeelding) VALUES (@Naam, @Prijs, @Afbeelding)";
                        command.Parameters.AddWithValue("@Naam", Product.Naam);
                        command.Parameters.AddWithValue("@Prijs", Product.Prijs);
                        command.Parameters.AddWithValue("@Afbeelding", fileName);

                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                    return RedirectToPage("/Beheer");
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