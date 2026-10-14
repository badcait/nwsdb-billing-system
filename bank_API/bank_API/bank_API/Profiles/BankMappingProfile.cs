using AutoMapper;
using bank_API.Model;
using bank_API.DTO;
namespace bank_API.Profiles;
public class BankMappingProfile : Profile
{
    public BankMappingProfile() => CreateMap<BankTransaction, BankPaymentResultDto>();
}
