using AutoMapper;
using GroupsApp.Api.Data;
using GroupsApp.Api.DTOs;
using GroupsApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var groups = await _db.Groups
                .Include(g => g.GroupMembers)
                .Include(g => g.Transactions)
                .ToListAsync();

            return groups.Select(g => _mapper.Map<GroupDto>(g));
        }

        public async Task<GroupDetailDto> GetByIdAsync(int groupId)
        {
            var group = await _db.Groups
                .Include(g => g.GroupMembers).ThenInclude(gm => gm.User)
                .Include(g => g.Transactions)
                .FirstOrDefaultAsync(g => g.Id == groupId)
                ?? throw new KeyNotFoundException("Group not found");

            var dto = _mapper.Map<GroupDetailDto>(group);

            dto.Members = group.GroupMembers
                .Select(gm => new MemberDto
                {
                    Id      = gm.User.Id,
                    Name    = gm.User.Name,
                    Balance = CalculateUserBalanceInGroup(gm.User.Id, groupId)
                })
                .ToList();

            dto.Transactions = group.Transactions
                .Select(tx => _mapper.Map<TransactionDto>(tx))
                .ToList();

            return dto;
        }

        public async Task<IEnumerable<MemberDto>> GetMembersAsync(int groupId)
        {
            var members = await _db.GroupMembers
                .Include(gm => gm.User)
                .Where(gm => gm.GroupId == groupId)
                .ToListAsync();

            return members
                .Select(gm => new MemberDto
                {
                    Id      = gm.User.Id,
                    Name    = gm.User.Name,
                    Balance = CalculateUserBalanceInGroup(gm.User.Id, groupId)
                });
        }

        public async Task<IEnumerable<TransactionDto>> GetGroupTransactionsAsync(int groupId)
        {
            var transactions = await _db.Transactions
                .Where(t => t.GroupId == groupId)
                .ToListAsync();

            return transactions.Select(tx => _mapper.Map<TransactionDto>(tx));
        }

        public async Task AddMemberAsync(int groupId, CreateMemberDto dto)
        {
            var user = new User { Name = dto.Name };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var group = await _db.Groups.FindAsync(groupId)
                ?? throw new KeyNotFoundException("Group not found");

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
                .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId)
                ?? throw new KeyNotFoundException("Membership not found");

            _db.GroupMembers.Remove(membership);
            await _db.SaveChangesAsync();
        }

        public async Task<TransactionDto> CreateTransactionAsync(int groupId, CreateTransactionDto dto)
        {
            // validate payer
            if (!await _db.GroupMembers.AnyAsync(gm => gm.GroupId == groupId && gm.UserId == dto.PayerId))
                throw new InvalidOperationException("Payer is not a member of the group");

            // get member IDs
            var memberIds = await _db.GroupMembers
                .Where(gm => gm.GroupId == groupId)
                .Select(gm => gm.UserId)
                .ToListAsync();

            // prepare splits
            Dictionary<int, decimal> splits;
            if (dto.SplitType == SplitType.Equal)
            {
                var share = dto.Amount / memberIds.Count;
                splits = memberIds.ToDictionary(id => id, id => share);
            }
            else
            {
                splits = dto.SplitDetails != null
                    ? new Dictionary<int, decimal>(dto.SplitDetails)
                    : new Dictionary<int, decimal>();
            }

            var tx = new Transaction
            {
                GroupId      = groupId,
                PayerUserId  = dto.PayerId,
                Amount       = dto.Amount,
                Date         = DateTime.UtcNow,
                SplitDetails = splits
            };

            _db.Transactions.Add(tx);
            await _db.SaveChangesAsync();

            return _mapper.Map<TransactionDto>(tx);
        }

        private decimal CalculateUserBalanceInGroup(int userId, int groupId)
        {
            var txs = _db.Transactions
                .Where(t => t.GroupId == groupId)
                .AsEnumerable();

            decimal balance = 0m;
            foreach (var tx in txs)
            {
                if (tx.SplitDetails != null &&
                    tx.SplitDetails.TryGetValue(userId, out var share))
                {
                    balance += (tx.PayerUserId == userId)
                        ? tx.Amount - share
                        : -share;
                }
            }

            return balance;
        }
    }
}
