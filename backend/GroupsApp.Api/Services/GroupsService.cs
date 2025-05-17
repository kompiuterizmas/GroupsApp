// backend/GroupsApp.Api/Services/GroupsService.cs

using AutoMapper;
using GroupsApp.Api.Data;
using GroupsApp.Api.DTOs;
using GroupsApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GroupsApp.Api.Services
{
    public class GroupsService : IGroupsService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public GroupsService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<GroupDto> CreateAsync(CreateGroupDto dto)
        {
            var group = new Group { Title = dto.Title };
            _db.Groups.Add(group);
            await _db.SaveChangesAsync();
            return _mapper.Map<GroupDto>(group);
        }

        public async Task<IEnumerable<GroupDto>> GetAllAsync()
        {
            var groups = await _db.Groups.ToListAsync();
            return groups.Select(g => _mapper.Map<GroupDto>(g));
        }

        public async Task<GroupDetailDto> GetByIdAsync(int groupId)
        {
            var group = await _db.Groups
                .Include(g => g.GroupMembers)
                    .ThenInclude(gm => gm.User)
                .Include(g => g.Transactions)
                .FirstOrDefaultAsync(g => g.Id == groupId)
                ?? throw new KeyNotFoundException("Group not found");

            var dto = _mapper.Map<GroupDetailDto>(group);

            // Map members with their per-group balance
            dto.Members = group.GroupMembers
                .Select(gm => new MemberDto
                {
                    Id      = gm.User.Id,
                    Name    = gm.User.Name,
                    Balance = CalculateUserBalanceInGroup(gm.User.Id, groupId)
                })
                .ToList();

            // Map transactions
            dto.Transactions = group.Transactions
                .Select(tx => _mapper.Map<TransactionDto>(tx))
                .ToList();

            return dto;
        }

        public async Task AddMemberAsync(int groupId, CreateMemberDto dto)
        {
            // Create user account
            var user = new User { Name = dto.Name };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // Load group for navigation property
            var group = await _db.Groups.FindAsync(groupId)
                ?? throw new KeyNotFoundException("Group not found");

            // Create join record with both FK and navigation props
            var membership = new GroupMember
            {
                GroupId = groupId,
                Group   = group,
                UserId  = user.Id,
                User    = user
            };

            _db.GroupMembers.Add(membership);
            await _db.SaveChangesAsync();
        }

        public async Task RemoveMemberAsync(int groupId, int userId)
        {
            var membership = await _db.GroupMembers
                .Include(gm => gm.Group)
                .Include(gm => gm.User)
                .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId)
                ?? throw new KeyNotFoundException("Membership not found");

            _db.GroupMembers.Remove(membership);
            await _db.SaveChangesAsync();
        }

        public async Task<TransactionDto> CreateTransactionAsync(int groupId, CreateTransactionDto dto)
        {
            // Validate group and payer exist
            var group = await _db.Groups.FindAsync(groupId)
                ?? throw new KeyNotFoundException("Group not found");
            var payer = await _db.Users.FindAsync(dto.PayerId)
                ?? throw new KeyNotFoundException("Payer not found");

            var tx = new Transaction
            {
                GroupId      = groupId,
                Group        = group,
                PayerUserId  = dto.PayerId,
                Payer        = payer,
                Amount       = dto.Amount,
                Date         = DateTime.UtcNow,
                SplitDetails = dto.SplitDetails
            };

            _db.Transactions.Add(tx);
            await _db.SaveChangesAsync();

            return _mapper.Map<TransactionDto>(tx);
        }

        // Helper: calculates net balance for a user in a group
        private decimal CalculateUserBalanceInGroup(int userId, int groupId)
        {
            // Fetch all transactions in this group
            var txs = _db.Transactions
                .Where(t => t.GroupId == groupId)
                .AsEnumerable(); // so we can read SplitDetails dictionary

            decimal balance = 0m;
            foreach (var tx in txs)
            {
                if (tx.SplitDetails != null && tx.SplitDetails.TryGetValue(userId, out var share))
                {
                    if (tx.PayerUserId == userId)
                        balance += tx.Amount - share;
                    else
                        balance -= share;
                }
            }
            return balance;
        }
    }
}
