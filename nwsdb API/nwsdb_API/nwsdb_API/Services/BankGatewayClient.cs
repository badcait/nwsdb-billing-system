using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using nwsdb_API.DTO;
namespace nwsdb_API.Services;
public class BankGatewayClient(HttpClient http)
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };
    public async Task<BankPaymentResultDto> AuthorizeAsync(BankPaymentRequestDto request)
    {
        using var response = await http.PostAsJsonAsync("api/bank/payments", request, Json);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BankPaymentResultDto>(Json) ?? throw new HttpRequestException("Empty bank response.");
    }
    public async Task<BankPaymentResultDto?> GetResultAsync(Guid id)
    {
        using var response = await http.GetAsync("api/bank/payments/" + id);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BankPaymentResultDto>(Json) ?? throw new HttpRequestException("Empty bank response.");
    }
}
