using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Diagnostics;

namespace Webshop_089667.Pages
{
    public class BeheerModel : PageModel
    {
        SqliteConnection connection;
        public List<Product> Products = new List<Product>();
        [BindProperty(SupportsGet = true)]
        public int artikelnummer { get; set; }

        public BeheerModel()
        {
            SqliteConnectionStringBuilder connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "motoren.db";
            connection = new SqliteConnection(connectionStringBuilder.ToString());
        }

        public IActionResult OnGet(string? logout)
        {
            if (HttpContext.Session.GetString("AdminIngelogd") != "true")
            {
                return RedirectToPage("/Index");
            }
            else
            {
                if (!string.IsNullOrEmpty(logout) && logout.ToLower() == "true")
                {
                    HttpContext.Session.Remove("AdminIngelogd");
                    Response.Redirect("/Index");
                }

                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM producten";
                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Product product = new Product
                    {
                        Artikelnummer = reader.GetInt32(0),
                        Naam = reader.GetString(1),
                        Prijs = (decimal)reader.GetDouble(2),
                        Afbeelding = reader.GetString(3)
                    };

                    Products.Add(product);
                }

                if (artikelnummer != 0)
                {
                    SqliteCommand deleteCommand = connection.CreateCommand();
                    deleteCommand.CommandText = $"DELETE FROM producten WHERE ID = {artikelnummer}";
                    deleteCommand.ExecuteReader();

                    return RedirectToPage("/Beheer");
                }

                connection.Close();
                return Page();
            }
        }
    }
}