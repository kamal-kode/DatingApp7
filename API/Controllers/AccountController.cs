using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController : BaseAPIController
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.UserName))
            return BadRequest("Username is taken");

        var newUser = _mapper.Map<AppUser>(registerDto);
        newUser.UserName = registerDto.UserName.ToLower();
        //Hash password
        // using var hmac = new HMACSHA512();

        newUser.UserName = registerDto.UserName;
        // newUser.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
        // newUser.PasswordSalt = hmac.Key;

        //Create new user with identity
        var result = await _userManager.CreateAsync(newUser, registerDto.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);
        
        //Add role to new user
        var roleResult = await _userManager.AddToRoleAsync(newUser, "Member");
        
        if (!roleResult.Succeeded)
            return BadRequest(roleResult.Errors);
        return new UserDto
        {
            UserName = registerDto.UserName,
            Token = await _tokenService.CreateToken(newUser),
            KnownAs = newUser.KnownAs,
            Gender = newUser.Gender
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _userManager.Users
        .Include(p => p.photos).SingleOrDefaultAsync(x => x.UserName == loginDto.UserName);

        if (user == null)
            return Unauthorized("invalid username");

        //We will be using asp.net identity
        // using var hmac = new HMACSHA512(user.PasswordSalt);
        // var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        // var result = computedHash.SequenceEqual(user.PasswordHash);
        // if (result == false) return Unauthorized("Invalid password");
        // for (int i = 0; i < computedHash.Length; i++)
        // {
        //     if (computedHash[i] != user.PasswordHash[i])
        //         return Unauthorized("Invalid password");
        // }
        var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

        if (!result)
            return Unauthorized("invalid password");

        return new UserDto
        {
            UserName = loginDto.UserName,
            Token = await _tokenService.CreateToken(user),
            PhotoUrl = user.photos.FirstOrDefault(x => x.IsMain)?.Url,
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await _userManager.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }
}
