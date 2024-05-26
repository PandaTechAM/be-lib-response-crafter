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
        throw new BadRequestException("some_exception");
    }

    [HttpGet("/error-by-result-controller")]
    public IActionResult ErrorByResult()
    {
        return BadRequest("some_exception");
    }
    
    [HttpPost("/load-controller")]
    public async Task<IActionResult> Load(TestDto dto)
    {
        await Task.Delay(77);
        return Ok(dto);
    }
}