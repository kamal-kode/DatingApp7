using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController : BaseAPIController
{
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
    {
        _context = context;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.UserName))
            return BadRequest("Username is taken");
        var newUser = _mapper.Map<AppUser>(registerDto);
        //Hash password
        using var hmac = new HMACSHA512();

        newUser.UserName = registerDto.UserName;
        newUser.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
        newUser.PasswordSalt = hmac.Key;

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        return new UserDto
        {
            UserName = registerDto.UserName,
            Token = _tokenService.CreateToken(newUser),
            KnownAs = newUser.KnownAs,
            Gender = newUser.Gender
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _context.Users
        .Include(p => p.photos).SingleOrDefaultAsync(x => x.UserName == loginDto.UserName);
        if (user == null) return Unauthorized();
        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        var result = computedHash.SequenceEqual(user.PasswordHash);
        if (result == false) return Unauthorized("Invalid password");
        // for (int i = 0; i < computedHash.Length; i++)
        // {
        //     if (computedHash[i] != user.PasswordHash[i])
        //         return Unauthorized("Invalid password");
        // }
        return new UserDto
        {
            UserName = loginDto.UserName,
            Token = _tokenService.CreateToken(user),
            PhotoUrl = user.photos.FirstOrDefault(x => x.IsMain)?.Url,
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await _context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }
}
