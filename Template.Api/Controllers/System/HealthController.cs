using Microsoft.AspNetCore.Mvc;

namespace Template.Api.Controllers.System;

[Route("api/[controller]")]
[ApiController]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Endpoint de verificação de integridade.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult HealthCheck()
    {
        return Ok(true);
    }
}
