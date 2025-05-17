using AutoMapper;
using GroupsApp.Api.DTOs;
using GroupsApp.Api.Models;
using System.Linq;

namespace GroupsApp.Api.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Group → GroupDto
            CreateMap<Group, GroupDto>()
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src =>
                    // suma balansų per visus narius grupėje
                    src.GroupMembers.Sum(gm =>
                        CalculateBalanceForMember(gm.UserId, src.Id, gm.Group, gm.User)))
                );

            // Group → GroupDetailDto (Title + Id, navigacijos mapinamos žemiau)
            CreateMap<Group, GroupDetailDto>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Id,    opt => opt.MapFrom(src => src.Id));

            // Transaction → TransactionDto
            CreateMap<Transaction, TransactionDto>();

            // User → MemberDto (naudojamas atskiroje map’inimo logikoje)
            CreateMap<User, MemberDto>()
                .ForMember(dest => dest.Id,      opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name,    opt => opt.MapFrom(src => src.Name))
                // Balance bus apskaičiuotas per service, tad čia nebereikalingas
                .ForMember(dest => dest.Balance, opt => opt.Ignore());
        }

        // Helper within profile – you can move this to service if norite
        private decimal CalculateBalanceForMember(int userId, int groupId, Group group, User user)
        {
            // Čia tik pavyzdys – tikras skaičiavimas vyksta service sluoksnyje
            return group.Transactions.Sum(tx =>
                tx.SplitDetails != null && tx.SplitDetails.TryGetValue(userId, out var share)
                    ? (tx.PayerUserId == userId ? tx.Amount - share : -share)
                    : 0m
            );
        }
    }
}
