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
            // Group → GroupDto (include members)
            CreateMap<Group, GroupDto>()
                .ForMember(d => d.Balance,
                           opt => opt.MapFrom(src => CalculateGroupBalance(src)))
                .ForMember(d => d.Members,
                           opt => opt.MapFrom(src => src.GroupMembers));

            // Group → GroupDetailDto (service fills members & transactions)
            CreateMap<Group, GroupDetailDto>();

            // GroupMember → MemberDto
            CreateMap<GroupMember, MemberDto>()
                .ForMember(d => d.Id,
                           opt => opt.MapFrom(src => src.UserId))
                .ForMember(d => d.Name,
                           opt => opt.MapFrom(src => src.User!.Name))
                .ForMember(d => d.Balance,
                           opt => opt.Ignore());

            // Transaction → TransactionDto
            CreateMap<Transaction, TransactionDto>()
                .ForMember(d => d.PayerId,
                           opt => opt.MapFrom(src => src.PayerUserId))
                .ForMember(d => d.SplitType,
                           opt => opt.MapFrom(src => src.SplitType.ToString()))
                .ForMember(d => d.GroupId,
                           opt => opt.MapFrom(src => src.GroupId))
                .ForMember(d => d.Description,
                           opt => opt.MapFrom(src => src.Description));

            // User → UserDto
            CreateMap<User, UserDto>();
        }

        private decimal CalculateGroupBalance(Group group)
        {
            var balances = group.GroupMembers.ToDictionary(gm => gm.UserId, _ => 0m);
            foreach (var tx in group.Transactions)
            {
                if (tx.SplitDetails == null) continue;
                foreach (var kvp in tx.SplitDetails)
                {
                    if (!balances.ContainsKey(kvp.Key)) continue;
                    balances[kvp.Key] +=
                        tx.PayerUserId == kvp.Key
                        ? tx.Amount - kvp.Value
                        : -kvp.Value;
                }
            }
            return balances.Values.Sum();
        }
    }
}
