using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GroupsApp.Api.Services;
using GroupsApp.Api.DTOs;

namespace GroupsApp.Api.Controllers
{
    [ApiController]
    [Route("api/groups")]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupsService _service;

        public GroupsController(IGroupsService service) => _service = service;

        [HttpGet]
        public async Task<IEnumerable<GroupDto>> GetAll() => await _service.GetAllAsync();

        [HttpGet("{id}")]
        public async Task<GroupDetailDto> GetById(int id) => await _service.GetByIdAsync(id);

        [HttpPost]
        public async Task<ActionResult<GroupDto>> Create(CreateGroupDto dto)
        {
            var group = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = group.Id }, group);
        }

        [HttpGet("{id}/members")]
        public async Task<IEnumerable<MemberDto>> GetMembers(int id) =>
            await _service.GetMembersAsync(id);

        [HttpGet("{id}/transactions")]
        public async Task<IEnumerable<TransactionDto>> GetTransactions(int id) =>
            await _service.GetGroupTransactionsAsync(id);

        [HttpPost("{id}/members")]
        public async Task<IActionResult> AddMember(int id, CreateMemberDto dto)
        {
            await _service.AddMemberAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}/members/{userId}")]
        public async Task<IActionResult> RemoveMember(int id, int userId)
        {
            await _service.RemoveMemberAsync(id, userId);
            return NoContent();
        }

        [HttpPost("{id}/transactions")]
        public async Task<ActionResult<TransactionDto>> CreateTransaction(int id, CreateTransactionDto dto)
        {
            var tx = await _service.CreateTransactionAsync(id, dto);
            return CreatedAtAction(nameof(GetTransactions), new { id }, tx);
        }
    }
}