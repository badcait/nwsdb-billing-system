namespace nwsdb_API.Model;
public class PaymentConflictException(string message, string code) : Exception(message)
{
    public string Code { get; } = code;
}
