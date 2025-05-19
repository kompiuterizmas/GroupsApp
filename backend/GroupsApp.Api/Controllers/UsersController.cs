using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using GroupsApp.Api.Data;
using GroupsApp.Api.DTOs;

namespace GroupsApp.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public UsersController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<UserDto>> GetAll()
        {
            var users = await _db.Users.ToListAsync();
            return _mapper.Map<List<UserDto>>(users);
        }

        [HttpGet("{userId}/groups")]
        public async Task<IEnumerable<GroupDto>> GetUserGroups(int userId)
        {
            var groups = await _db.GroupMembers
                .Where(gm => gm.UserId == userId)
                .Select(gm => gm.Group)
                .ToListAsync();
            return _mapper.Map<List<GroupDto>>(groups);
        }

        [HttpGet("{userId}/transactions")]
        public async Task<IEnumerable<TransactionDto>> GetUserTransactions(int userId)
        {
            var txs = await _db.Transactions
                .Where(t => t.PayerUserId == userId)
                .ToListAsync();
            return _mapper.Map<List<TransactionDto>>(txs);
        }
    }
}