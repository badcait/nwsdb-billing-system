using System;
namespace desktop_client.DTO
{
public enum UserRole { Customer, Staff, PartnerCashier }
public enum AttemptStatus { Pending, Declined, Completed }
public class LoginRequestDto
{

    public string Username { get; set; } = "";

    public string Password { get; set; } = "";
}

public class LoginResponseDto
{

    public string AccessToken { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
    public UserRole Role { get; set; }
    public int UserId { get; set; }
    public int? PartnerId { get; set; }
}

public class BillLookupDto
{
    public int BillId { get; set; }

    public string AccountNumber { get; set; } = "";

    public string MaskedCustomerName { get; set; } = "";
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime DueDate { get; set; }
    public decimal Amount { get; set; }
    public bool IsProcessing { get; set; }
}

public class CreatePaymentDto
{
    public int BillId { get; set; }
    public decimal Amount { get; set; }
    public Guid ClientReference { get; set; }

    public string MockPaymentToken { get; set; } = "";
}

public class PaymentStatusDto
{
    public Guid AttemptId { get; set; }
    public Guid ClientReference { get; set; }
    public int BillId { get; set; }

    public string AccountNumber { get; set; } = "";
    public decimal Amount { get; set; }

    public string Currency { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public AttemptStatus Status { get; set; }

    public string Message { get; set; } = "";
    public ReceiptDto Receipt { get; set; }
}

public class ReceiptDto
{

    public string ReceiptNumber { get; set; } = "";

    public string AccountNumber { get; set; } = "";
    public int BillId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; }

    public string BankReference { get; set; } = "";
}

}
