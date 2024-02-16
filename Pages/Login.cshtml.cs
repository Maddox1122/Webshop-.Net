using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Webshop_089667.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Gebruikersnaam { get; set; }

        [BindProperty]
        public string Wachtwoord { get; set; }

        public string Foutmelding { get; private set; }

        public IActionResult OnPost()
        {
            if (Gebruikersnaam == "admin" && Wachtwoord == "#1Geheim")
            {
                HttpContext.Session.SetString("AdminIngelogd", "true");

                return RedirectToPage("Beheer");
            }
            else
            {
                Foutmelding = "Onjuiste gebruikersnaam of wachtwoord.";
                return Page();
            }
        }
    }
}
