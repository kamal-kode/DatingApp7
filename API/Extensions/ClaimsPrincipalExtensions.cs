using System.Security.Claims;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace API.Extensions;

public static class ClaimsPrincipalExtensions
{
  public static string GetUserName(this ClaimsPrincipal user){
    return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
  }
}
