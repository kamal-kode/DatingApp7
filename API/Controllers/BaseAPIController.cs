using Microsoft.AspNetCore.Mvc;
namespace API.Controllers;

/// <summary>
/// Api controller will automatically bind the body prams no need to define [FromBody] at action level
/// and if we add annotation it will automatically handles the errors no need to check for Model Valid property
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BaseAPIController : ControllerBase
{

}
