using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using website.DTO;
using website.Services;

namespace website.Pages;

public class StaffModel(NwsdbWebApiClient api) : PageModel
{
    public CustomerReadDto[] Customers { get; set; } = [];
    public AccountReadDto[] Accounts { get; set; } = [];
    public UsageReadDto[] Readings { get; set; } = [];
    public BillReadDto[] Bills { get; set; } = [];
    public PaymentStatusDto[] Payments { get; set; } = [];
    public int AccountId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? CustomerSearch { get; set; } = "";

    [BindProperty(SupportsGet = true)]
    public string? AccountSearch { get; set; } = "";

    [BindProperty(SupportsGet = true)]
    public int ResultPage { get; set; } = 1;

    [TempData]
    public string? Message { get; set; }

    public async Task<IActionResult> OnGetAsync(int accountId = 0)
    {
        if (HttpContext.Session.GetString("Role") != "Staff")
            return RedirectToPage("/Login");
        try
        {
            ResultPage = Math.Clamp(ResultPage, 1, 1000000);
            Customers = await api.GetCustomersAsync(CustomerSearch ?? "", ResultPage);
            Accounts = await api.GetAccountsAsync(AccountSearch ?? "", ResultPage);
            Payments = await api.GetPaymentsAsync();
            AccountId = accountId > 0 ? accountId : Accounts.FirstOrDefault()?.AccountId ?? 0;
            if (AccountId > 0)
            {
                Readings = await api.GetReadingsAsync(AccountId);
                Bills = await api.GetBillsAsync(AccountId);
            }
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            Message = "Cannot load staff data. Check the API and database, or sign in again.";
        }

        return Page();
    }

    private async Task<IActionResult> RunAsync(Func<Task> action, int id = 0)
    {
        if (HttpContext.Session.GetString("Role") != "Staff")
            return RedirectToPage("/Login");
        if (!ModelState.IsValid)
        {
            Message = "Check the entered values.";
            return RedirectToPage(new { accountId = id, CustomerSearch, AccountSearch, ResultPage });
        }

        try
        {
            Message = null;
            await action();
            Message ??= "Saved successfully.";
        }
        catch (HttpRequestException ex)
        {
            Message = ex.Message;
        }
        catch (TaskCanceledException)
        {
            Message = "Request timed out. Check the saved result before submitting again.";
        }

        return RedirectToPage(new { accountId = id, CustomerSearch, AccountSearch, ResultPage });
    }

    public Task<IActionResult> OnPostCustomerAsync(string fullName, string email, string phone) =>
        RunAsync(async () =>
    {
        await api.CreateCustomerAsync(new() { FullName = fullName, Email = email, Phone = phone });
    });

    public Task<IActionResult> OnPostAccountAsync(int customerId, string accountNumber, string serviceAddress) =>
        RunAsync(async () =>
    {
        await api.CreateAccountAsync(new() { CustomerId = customerId, AccountNumber = accountNumber, ServiceAddress = serviceAddress });
    });

    public Task<IActionResult> OnPostReadingAsync(int accountId, DateTime readingDate, decimal meterValue) =>
        RunAsync(async () =>
    {
        await api.RecordReadingAsync(accountId, new() { ReadingDate = readingDate, MeterValue = meterValue });
    }, accountId);

    public Task<IActionResult> OnPostBillAsync(int accountId, int startReadingId, int endReadingId, DateTime dueDate) =>
        RunAsync(async () =>
    {
        await api.GenerateBillAsync(accountId, new() { StartReadingId = startReadingId, EndReadingId = endReadingId, DueDate = dueDate });
    }, accountId);

    public Task<IActionResult> OnPostReconcileAsync(Guid id) =>
        RunAsync(async () =>
    {
        var status = await api.ReconcileAsync(id);
        Message = status.Message;
    });

    public Task<IActionResult> OnPostRemoveCustomerAsync(int customerId) =>
        RunAsync(async () =>
    {
        await api.RemoveCustomerAsync(customerId);
        Message = "Customer removed.";
    });

    public Task<IActionResult> OnPostRemoveAccountAsync(int accountId) =>
        RunAsync(async () =>
    {
        await api.RemoveAccountAsync(accountId);
        Message = "Water account removed.";
    });
}
