using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{
    private const string Member = "Member";
    private const string Admin = "Admin";
    private const string Moderator = "Moderator";
    public static async Task ClearConnections(DataContext dataContext)
    {
        dataContext.Connections.RemoveRange(dataContext.Connections);
        await dataContext.SaveChangesAsync();
    }
    public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        if (await userManager.Users.AnyAsync()) return;
        var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);

        //Add roles
        var roles = new List<AppRole>{
        new() { Name = Member},
        new() { Name = Admin},
        new() { Name = Moderator}
        };
        foreach (var role in roles)
        {
            await roleManager.CreateAsync(role);
        }

        //Add user data
        foreach (var user in users)
        {
            // using var hmac = new HMACSHA512();
            user.UserName = user.UserName.ToLower();
            user.Created = DateTime.SpecifyKind(user.Created, DateTimeKind.Utc);
            user.LastActive = DateTime.SpecifyKind(user.LastActive, DateTimeKind.Utc);
            // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
            // user.PasswordSalt = hmac.Key;
            await userManager.CreateAsync(user, "Pa$$w0rd");
            //Add default role
            await userManager.AddToRoleAsync(user, Member);
        }

        var admin = new AppUser
        {
            UserName = "admin"
        };
        await userManager.CreateAsync(admin, "Pa$$w0rd");
        //Add user with multiple role.
        await userManager.AddToRolesAsync(admin, new[] { Admin, Moderator });

    }
}
