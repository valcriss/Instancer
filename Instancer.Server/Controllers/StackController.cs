using Instancer.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Instancer.Server.Dtos;

namespace Instancer.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StackController : ControllerBase
    {
        private readonly StackService _stackService;

        public StackController(StackService stackService)
        {
            _stackService = stackService;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_stackService.GetAll());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStackRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Compose))
                return BadRequest("Name and compose are required.");

            var url = await _stackService.CreateAndDeployAsync(request.Name, request.Compose);
            var instance = _stackService.GetAll().FirstOrDefault(x => url.Contains(x.Port.ToString()));

            return Ok(new
            {
                accessUrl = url,
                port = instance?.Port,
                stackId = instance?.Id,
                name = instance?.Name
            });
        }

    }
}
