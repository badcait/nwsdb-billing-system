using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace website.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet() =>
        RedirectToPage(HttpContext.Session.GetString("Role") == "Staff" ? "/Staff" : HttpContext.Session.GetString("Role") == "Customer" ? "/Dashboard" : "/Login");
}
