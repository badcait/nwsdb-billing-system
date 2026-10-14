using System;
namespace desktop_client.Model
{
    public class PaymentDraft
    {
        public int OwnerUserId { get; set; }
        public int OwnerPartnerId { get; set; }
        public int BillId { get; set; }
        public decimal Amount { get; set; }
        public Guid ClientReference { get; set; }
        public string MockPaymentToken { get; set; }
    }
}
