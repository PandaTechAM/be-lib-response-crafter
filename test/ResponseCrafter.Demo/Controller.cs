using Microsoft.AspNetCore.Mvc;
using ResponseCrafter.HttpExceptions;

namespace ResponseCrafter.Demo;

[ApiController]
[Route("")]
public class Controller : ControllerBase
{
    [HttpGet("/error-by-exception-controller")]
    public IActionResult ErrorByException()
    {
        string? vazgen = null;
        
        NotFoundException.ThrowIfNull(vazgen);

        throw new BadRequestException("some_exception");
    }

    [HttpGet("/error-by-result-controller")]
    public IActionResult ErrorByResult()
    {
        return BadRequest("some_exception");
    }
}