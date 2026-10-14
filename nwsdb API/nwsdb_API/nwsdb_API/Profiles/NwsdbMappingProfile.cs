using AutoMapper;
using nwsdb_API.DTO;
using nwsdb_API.Model;
namespace nwsdb_API.Profiles;
public class NwsdbMappingProfile : Profile
{
    public NwsdbMappingProfile()
    {
        CreateMap<CreateCustomerDto, Customer>(MemberList.Source);
        CreateMap<Customer, CustomerReadDto>();
        CreateMap<CreateAccountDto, WaterAccount>(MemberList.Source);
        CreateMap<WaterAccount, AccountReadDto>();
        CreateMap<Bill, BillReadDto>()
            .ForMember(d => d.OutstandingAmount, o => o.MapFrom(s => s.Payment == null ? s.Amount : 0))
            .ForMember(d => d.IsProcessing, o => o.MapFrom(s => s.Attempts.Any(a => a.Status == AttemptStatus.Pending)));
        CreateMap<Payment, ReceiptDto>().ForMember(d => d.AccountNumber, o => o.MapFrom(s => s.Bill.Account.AccountNumber));
        CreateMap<PaymentAttempt, PaymentStatusDto>()
            .ForMember(d => d.AccountNumber, o => o.MapFrom(s => s.Bill.Account.AccountNumber))
            .ForMember(d => d.Receipt, o => o.MapFrom(s => s.Payment))
            .ForMember(d => d.Message, o => o.MapFrom(s => s.Status == AttemptStatus.Completed ? "Payment completed." : s.Status == AttemptStatus.Declined ? "Payment declined." : "Payment pending. Recheck using the original reference."));
    }
}
