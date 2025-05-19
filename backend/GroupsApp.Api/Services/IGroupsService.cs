using System.Collections.Generic;
using System.Threading.Tasks;
using GroupsApp.Api.DTOs;

namespace GroupsApp.Api.Services
{
    public interface IGroupsService
    {
        Task<IEnumerable<GroupDto>> GetAllAsync();

        // Changed return type from GroupDto to GroupDetailDto
        Task<GroupDetailDto> GetByIdAsync(int groupId);

        Task<GroupDto> CreateAsync(CreateGroupDto dto);
        Task AddMemberAsync(int groupId, CreateMemberDto dto);
        Task RemoveMemberAsync(int groupId, int userId);
        Task<TransactionDto> CreateTransactionAsync(int groupId, CreateTransactionDto dto);
        Task<IEnumerable<MemberDto>> GetMembersAsync(int groupId);
        Task<IEnumerable<TransactionDto>> GetGroupTransactionsAsync(int groupId);
    }
}
