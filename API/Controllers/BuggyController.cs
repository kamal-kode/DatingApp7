using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController : BaseAPIController
{
    private readonly DataContext _context;

    public BuggyController(DataContext context)
    {
        _context = context;
    }
    [HttpGet("auth")]
    [Authorize]
    public ActionResult<string> GetSecret()
    {
        return "secret text";
    }
    [HttpGet("not-found")]
    public ActionResult<AppUser> NotFoundRequest()
    {
        var user = _context.Users.Find(-1);
        if (user == null) return NotFound();
        return user;
    }
    [HttpGet("server-error")]
    public ActionResult<string> GetServerError()
    {
        var user = _context.Users.Find(-1);
        var userfound = user.ToString();
        return userfound;
    }
    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest()
    {
        return BadRequest("Bad request found");
    }
}
