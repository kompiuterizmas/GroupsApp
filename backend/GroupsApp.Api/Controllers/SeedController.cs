using Microsoft.AspNetCore.Mvc;
using GroupsApp.Api.Services;

namespace GroupsApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]  // nerodyti Swagger’e
    public class SeedController : ControllerBase
    {
        private readonly ISeedService _seeder;

        public SeedController(ISeedService seeder)
        {
            _seeder = seeder;
        }

        [HttpPost]
        public IActionResult Seed()
        {
            _seeder.SeedDemoData();
            return NoContent();
        }
    }
}
