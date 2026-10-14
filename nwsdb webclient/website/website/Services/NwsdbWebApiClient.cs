using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using website.DTO;

namespace website.Services;

public class NwsdbWebApiClient(HttpClient http, IHttpContextAccessor accessor)
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    private async Task<T> SendAsync<T>(HttpMethod method, string path, object? body = null)
    {
        using var request = new HttpRequestMessage(method, path);
        var token = accessor.HttpContext?.Session.GetString("Token");
        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (body != null)
            request.Content = JsonContent.Create(body, options: Json);
        using var response = await http.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            string message = $"API returned {(int)response.StatusCode}.";
            try
            {
                using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                if (doc.RootElement.TryGetProperty("title", out var t))
                    message = t.GetString() ?? message;
            }
            catch (JsonException)
            {
            }

            throw new HttpRequestException(message, null, response.StatusCode);
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            return default!;
        return await response.Content.ReadFromJsonAsync<T>(Json) ?? throw new HttpRequestException("The API returned no data.");
    }

    public Task<LoginResponseDto> LoginAsync(LoginRequestDto r) =>
        SendAsync<LoginResponseDto>(HttpMethod.Post, "api/auth/login", r);

    public Task<AccountSummaryDto[]> GetMyAccountsAsync() =>
        SendAsync<AccountSummaryDto[]>(HttpMethod.Get, "api/me/accounts");

    public Task<UsageReadDto[]> GetReadingsAsync(int id) =>
        SendAsync<UsageReadDto[]>(HttpMethod.Get, $"api/me/accounts/{id}/readings");

    public Task<BillReadDto[]> GetBillsAsync(int id) =>
        SendAsync<BillReadDto[]>(HttpMethod.Get, $"api/me/accounts/{id}/bills");

    public Task<ReceiptDto[]> GetMyPaymentsAsync() =>
        SendAsync<ReceiptDto[]>(HttpMethod.Get, "api/me/payments");

    public Task<CustomerReadDto[]> GetCustomersAsync(string search = "", int page = 1) =>
        SendAsync<CustomerReadDto[]>(HttpMethod.Get, $"api/customers?search={Uri.EscapeDataString(search)}&page={page}");

    public Task<AccountReadDto[]> GetAccountsAsync(string search = "", int page = 1) =>
        SendAsync<AccountReadDto[]>(HttpMethod.Get, $"api/accounts?search={Uri.EscapeDataString(search)}&page={page}");

    public Task<CustomerReadDto> CreateCustomerAsync(CreateCustomerDto r) =>
        SendAsync<CustomerReadDto>(HttpMethod.Post, "api/customers", r);

    public Task<AccountReadDto> CreateAccountAsync(CreateAccountDto r) =>
        SendAsync<AccountReadDto>(HttpMethod.Post, "api/accounts", r);

    public Task<UsageReadDto> RecordReadingAsync(int id, CreateReadingDto r) =>
        SendAsync<UsageReadDto>(HttpMethod.Post, $"api/accounts/{id}/readings", r);

    public Task<BillReadDto> GenerateBillAsync(int id, GenerateBillDto r) =>
        SendAsync<BillReadDto>(HttpMethod.Post, $"api/accounts/{id}/bills", r);

    public Task<PaymentStatusDto[]> GetPaymentsAsync() =>
        SendAsync<PaymentStatusDto[]>(HttpMethod.Get, "api/payments");

    public Task<PaymentStatusDto> ReconcileAsync(Guid id) =>
        SendAsync<PaymentStatusDto>(HttpMethod.Post, $"api/payments/{id}/reconcile");

    public async Task RemoveCustomerAsync(int id) =>
        await SendAsync<object?>(HttpMethod.Delete, $"api/customers/{id}");

    public async Task RemoveAccountAsync(int id) =>
        await SendAsync<object?>(HttpMethod.Delete, $"api/accounts/{id}");
}
