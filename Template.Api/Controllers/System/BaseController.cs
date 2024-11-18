using Template.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Template.Api.Controllers.System;

public abstract class BaseController : ControllerBase
{
    protected IActionResult HandleResponse<T>(ApiResponse<T> response) where T : class
    {
        if (response.Success)
            return Ok(response);

        if (response is ErrorResponse<T> errorResponse)
            return StatusCode(errorResponse.StatusCode, errorResponse);

        return BadRequest(response);
    }
}