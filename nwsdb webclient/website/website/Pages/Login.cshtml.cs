using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using website.DTO;
using website.Services;

namespace website.Pages;

public class LoginModel(NwsdbWebApiClient api) : PageModel
{
    [BindProperty]
    public string Username { get; set; } = "";

    [BindProperty]
    public string Password { get; set; } = "";
    public string? Error { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Error = "Enter your username and password.";
            return Page();
        }

        try
        {
            var r = await api.LoginAsync(new LoginRequestDto { Username = Username, Password = Password });
            if (r.Role.ToString() == "PartnerCashier")
            {
                Error = "Use the Desktop Client for partner collections.";
                return Page();
            }

            HttpContext.Session.Clear();
            HttpContext.Session.SetString("Token", r.AccessToken);
            HttpContext.Session.SetString("Role", r.Role.ToString());
            return RedirectToPage(r.Role.ToString() == "Staff" ? "/Staff" : "/Dashboard");
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            Error = "Sign-in failed. Check your details and that the NWSDB API is running.";
            return Page();
        }
    }

    public IActionResult OnPostLogout()
    {
        HttpContext.Session.Clear();
        return RedirectToPage();
    }
}
