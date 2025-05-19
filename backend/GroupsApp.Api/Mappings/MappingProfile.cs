using AutoMapper;
using GroupsApp.Api.DTOs;
using GroupsApp.Api.Models;
using System.Collections.Generic;
using System.Linq;

namespace GroupsApp.Api.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map Group to GroupDto and compute overall balance
            CreateMap<Group, GroupDto>()
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src =>
                    CalculateGroupBalance(src)));

            // Map Group to GroupDetailDto (members & transactions are set in service)
            CreateMap<Group, GroupDetailDto>();

            // Map GroupMember to MemberDto
            CreateMap<GroupMember, MemberDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.Balance, opt => opt.Ignore());  // balance calculated in service

            // Map Transaction to TransactionDto
            CreateMap<Transaction, TransactionDto>();

            // Map User to UserDto
            CreateMap<User, UserDto>();
        }

        private decimal CalculateGroupBalance(Group group)
        {
            var balances = new Dictionary<int, decimal>();
            foreach (var gm in group.GroupMembers)
            {
                balances[gm.UserId] = 0m;
            }
            foreach (var tx in group.Transactions)
            {
                if (tx.SplitDetails is null) continue;
                foreach (var kvp in tx.SplitDetails)
                {
                    var userId = kvp.Key;
                    var share = kvp.Value;
                    if (!balances.ContainsKey(userId)) continue;
                    balances[userId] += tx.PayerUserId == userId ? tx.Amount - share : -share;
                }
            }
            // sum of balances (example: total collective balance)
            return balances.Values.Sum();
        }
    }
}