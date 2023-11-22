using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Now angular app in API so when API doesn't know what to do with the root it will use this control to handle fallback urls
/// </summary>
public class FallBackController : Controller
{
    public ActionResult Index()
    {
        return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"), "text/html");
    }
}
