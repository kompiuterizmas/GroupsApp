using Microsoft.AspNetCore.Mvc;
using GroupsApp.Api.Services;
using GroupsApp.Api.DTOs;

namespace GroupsApp.Api.Controllers
{
    [ApiController]
    [Route("api/groups/{groupId}/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly IGroupsService _service;

        public TransactionsController(IGroupsService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(int groupId, CreateTransactionDto dto)
        {
            var tx = await _service.CreateTransactionAsync(groupId, dto);
            return CreatedAtAction(nameof(Create), new { groupId = groupId, id = tx.Id }, tx);
        }
    }
}
