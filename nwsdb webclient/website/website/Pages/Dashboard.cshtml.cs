using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using website.DTO;
using website.Services;

namespace website.Pages;

public class DashboardModel(NwsdbWebApiClient api) : PageModel
{
    public AccountSummaryDto[] Accounts { get; set; } = [];
    public UsageReadDto[] Readings { get; set; } = [];
    public BillReadDto[] Bills { get; set; } = [];
    public ReceiptDto[] Receipts { get; set; } = [];
    public AccountSummaryDto? Selected { get; set; }
    public string? Error { get; set; }

    public async Task<IActionResult> OnGetAsync(int? accountId)
    {
        if (HttpContext.Session.GetString("Role") != "Customer")
            return RedirectToPage("/Login");
        try
        {
            Accounts = await api.GetMyAccountsAsync();
            Selected = Accounts.FirstOrDefault(x => x.AccountId == accountId) ?? Accounts.FirstOrDefault();
            if (Selected != null)
            {
                Readings = await api.GetReadingsAsync(Selected.AccountId);
                Bills = await api.GetBillsAsync(Selected.AccountId);
            }

            Receipts = await api.GetMyPaymentsAsync();
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            Error = "Could not load your account. Check the API connection or sign in again.";
        }

        return Page();
    }
}
