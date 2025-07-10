using Microsoft.AspNetCore.Mvc;

namespace TicTacToe.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok();
    }
}