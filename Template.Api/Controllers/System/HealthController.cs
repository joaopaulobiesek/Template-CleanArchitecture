using Microsoft.AspNetCore.Mvc;

namespace Template.Api.Controllers.System;

[Route("api/[controller]")]
[ApiController]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Verifica a integridade do sistema.
    /// </summary>
    /// <remarks>
    /// Este endpoint retorna um status indicando se a API está operacional.
    /// 
    /// **Regras de negócio:**
    /// - Sempre retorna `200 OK` caso o serviço esteja em funcionamento.
    /// - Pode ser utilizado para monitoramento de disponibilidade da API.
    /// </remarks>
    /// <returns>Retorna um status indicando que o sistema está saudável.</returns>
    /// <response code="200">O sistema está operacional.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult HealthCheck()
        => Ok(true);
}
