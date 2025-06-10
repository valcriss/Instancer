using Instancer.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Instancer.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TemplatesController : ControllerBase
    {
        private readonly TemplateService _templateService;

        public TemplatesController(TemplateService templateService)
        {
            _templateService = templateService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var templates = _templateService.GetTemplates();
            return Ok(templates);
        }
    }
}
