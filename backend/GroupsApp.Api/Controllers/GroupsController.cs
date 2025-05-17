using Microsoft.AspNetCore.Mvc;
using GroupsApp.Api.Services;
using GroupsApp.Api.DTOs;

namespace GroupsApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupsService _service;

        public GroupsController(IGroupsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var groups = await _service.GetAllAsync();
            return Ok(groups);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateGroupDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var group = await _service.GetByIdAsync(id);
            return Ok(group);
        }

        [HttpPost("{id}/members")]
        public async Task<IActionResult> AddMember(int id, CreateMemberDto dto)
        {
            await _service.AddMemberAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}/members/{memberId}")]
        public async Task<IActionResult> RemoveMember(int id, int memberId)
        {
            await _service.RemoveMemberAsync(id, memberId);
            return NoContent();
        }
    }
}
