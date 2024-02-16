using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace Webshop_089667.Pages
{
    public class IndexModel : PageModel
    {
        SqliteConnection connection;
        public List<Product> Products = new List<Product>();
        public IndexModel()
        {
            SqliteConnectionStringBuilder connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "motoren.db";
            connection = new SqliteConnection(connectionStringBuilder.ToString());
        }

        public void OnGet()
        {

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

            connection.Close();
        }
    }
}
