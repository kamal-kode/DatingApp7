using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Authorize]
public class UsersController : BaseAPIController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;

    public UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _photoService = photoService;
    }

    // [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<PageList<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
    {
        var gender = await _unitOfWork.UserRepository.GetUserGender(User.GetUserName());
        userParams.CurrentUserName = User.GetUserName();
        //If logged in user is male return female profiles
        if (string.IsNullOrEmpty(userParams.Gender))
        {
            userParams.Gender = gender == "male" ? "female" : "male";
        }
        var users = await _unitOfWork.UserRepository.GetMembersAsync(userParams);
        Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));
        return Ok(users);
    }

    [HttpGet("{username}")]
    // [Authorize(Roles = "Member")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        return await _unitOfWork.UserRepository.GetMemberAsync(username);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(User.GetUserName());
        if (user == null) return NotFound();

        _mapper.Map(memberUpdateDto, user);

        if (await _unitOfWork.Complete()) return NoContent();

        return BadRequest("Failed to update user");

    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(User.GetUserName());
        if (user == null) return NotFound();
        var result = await _photoService.AddPhotoAsync(file);
        if (result.Error != null) return BadRequest(result.Error.Message);
        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        if (user.photos.Count == 0) photo.IsMain = true;
        user.photos.Add(photo);
        /*CreatedAtAction - It will add Location key in headers and the value as newly created resource path.
        With 201 status code.*/
        if (await _unitOfWork.Complete())
            return CreatedAtAction(nameof(GetUser)
             , new { username = user.UserName }
             , _mapper.Map<PhotoDto>(photo));

        return BadRequest("Problem adding photos");
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(User.GetUserName());
        if (user == null) return NotFound();
        var photo = user.photos.FirstOrDefault(x => x.Id == photoId);
        if (photo == null) return NotFound("No photo available");

        if (photo.IsMain) return BadRequest("This is already your main photo");
        var currentMain = user.photos.FirstOrDefault(x => x.IsMain);
        if (currentMain != null) currentMain.IsMain = false;
        photo.IsMain = true;
        if (await _unitOfWork.Complete()) return NoContent();
        return BadRequest("Problem setting the main photo");
    }
    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(User.GetUserName());
        var photo = user.photos.FirstOrDefault(x => x.Id == photoId);
        if (photo == null) return NotFound();
        if (photo.IsMain) return BadRequest("You can not delete your main photo");
        if (photo.PublicId != null)
        {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);

        }
        user.photos.Remove(photo);
        if (await _unitOfWork.Complete()) return Ok();
        return BadRequest("Problem deleting photo");
    }
}
