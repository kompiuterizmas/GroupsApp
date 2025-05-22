using AutoMapper;
using GroupsApp.Api.Data;
using GroupsApp.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroupsApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public UsersController(AppDbContext db, IMapper mapper)
        {
            _db     = db;
            _mapper = mapper;
        }

        // GET /api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var users = await _db.Users.ToListAsync();
            return Ok(users.Select(u => _mapper.Map<UserDto>(u)));
        }

        // GET /api/users/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDto>> GetById(int userId)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return NotFound();
            return Ok(_mapper.Map<UserDto>(user));
        }

        // GET /api/users/{userId}/groups
        [HttpGet("{userId}/groups")]
        public async Task<ActionResult<IEnumerable<GroupDto>>> GetUserGroups(int userId)
        {
            // ensure user exists
            if (!await _db.Users.AnyAsync(u => u.Id == userId))
                return NotFound();

            // find group IDs the user belongs to
            var groupIds = await _db.GroupMembers
                .Where(gm => gm.UserId == userId)
                .Select(gm => gm.GroupId)
                .ToListAsync();

            // load those groups with their members *including* user nav property
            var groups = await _db.Groups
                .Where(g => groupIds.Contains(g.Id))
                .Include(g => g.GroupMembers)
                    .ThenInclude(gm => gm.User)    // <-- load User here!
                .Include(g => g.Transactions)
                .ToListAsync();

            // map to DTO
            return Ok(groups.Select(g => _mapper.Map<GroupDto>(g)));
        }

        // GET /api/users/{userId}/transactions
        [HttpGet("{userId}/transactions")]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetUserTransactions(int userId)
        {
            if (!await _db.Users.AnyAsync(u => u.Id == userId))
                return NotFound();

            var allTx = await _db.Transactions.ToListAsync();
            var userTx = allTx
                .Where(tx => tx.SplitDetails != null && tx.SplitDetails.ContainsKey(userId))
                .ToList();

            return Ok(userTx.Select(tx => _mapper.Map<TransactionDto>(tx)));
        }
    }
}
