using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using desktop_client.DTO;
namespace desktop_client.Services
{
    public class ApiException : Exception
    {
        public int StatusCode { get; private set; }
        public string Code { get; private set; }
        public ApiException(int code, string message, string errorCode = null) : base(message) { StatusCode = code; Code = errorCode; }
    }
    public class PartnerApiClient : IDisposable
    {
        private readonly HttpClient http;
        public PartnerApiClient(string url) { http = new HttpClient { BaseAddress = new Uri(url), Timeout = TimeSpan.FromSeconds(25) }; }
        private async Task<T> SendAsync<T>(HttpMethod method, string path, object body = null)
        {
            using (var request = new HttpRequestMessage(method, path))
            {
                if (body != null) request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                using (var response = await http.SendAsync(request))
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        var message = "API returned " + (int)response.StatusCode;
                        string errorCode = null;
                        try { var error = JObject.Parse(content); message = (string)error["title"] ?? message; errorCode = (string)error["code"]; } catch (JsonException) { }
                        throw new ApiException((int)response.StatusCode, message, errorCode);
                    }
                    return JsonConvert.DeserializeObject<T>(content);
                }
            }
        }
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var result = await SendAsync<LoginResponseDto>(HttpMethod.Post, "api/auth/login", request);
            if (result.Role != UserRole.PartnerCashier || !result.PartnerId.HasValue) throw new ApiException(403, "Use a partner cashier account.");
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
            return result;
        }
        public Task<BillLookupDto[]> LookupBillsAsync(string number) => SendAsync<BillLookupDto[]>(HttpMethod.Get, "api/partner/accounts/" + Uri.EscapeDataString(number) + "/bills");
        public Task<PaymentStatusDto> SubmitPaymentAsync(CreatePaymentDto request) => SendAsync<PaymentStatusDto>(HttpMethod.Post, "api/payments", request);
        public Task<PaymentStatusDto[]> GetHistoryAsync() => SendAsync<PaymentStatusDto[]>(HttpMethod.Get, "api/payments");
        public Task<PaymentStatusDto> GetStatusAsync(Guid id) => SendAsync<PaymentStatusDto>(HttpMethod.Get, "api/payments/" + id);
        public Task<PaymentStatusDto> ReconcileAsync(Guid id) => SendAsync<PaymentStatusDto>(HttpMethod.Post, "api/payments/" + id + "/reconcile");
        public Task<ReceiptDto> GetReceiptAsync(string number) => SendAsync<ReceiptDto>(HttpMethod.Get, "api/receipts/" + Uri.EscapeDataString(number));
        public void Dispose() { http.Dispose(); }
    }
}
