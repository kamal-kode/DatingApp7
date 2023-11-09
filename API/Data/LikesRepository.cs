using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API;

public class LikesRepository : ILikesRepository
{
    private readonly DataContext _context;

    public LikesRepository(DataContext context)
    {
        _context = context;
    }
    public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
    {
        return await _context.Likes.FindAsync(sourceUserId, targetUserId);
    }

    public async Task<PageList<LikeDto>> GetUserLikes(LikesParams likesParams)
    {
        var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
        var likes = _context.Likes.AsQueryable();
        if (likesParams.Predicate == "liked")
        {
            likes = likes.Where(l => l.SourceUserId == likesParams.UserId);
            users = likes.Select(like => like.TargetUser);
        }
        if (likesParams.Predicate == "likedBy")
        {
            likes = likes.Where(l => l.TargetUserId == likesParams.UserId);
            users = likes.Select(like => like.SourceUser);
        }
        var likedUsers= users.Select(user => new LikeDto
        {
            UserName = user.UserName,
            KnowAs = user.KnownAs,
            Age = user.DateOfBirth.CalculateAge(),
            PhotoUrl = user.photos.FirstOrDefault(x => x.IsMain).Url,
            City = user.City,
            Id = user.Id
        });
        return await PageList<LikeDto>.CreateAsync(likedUsers,likesParams.PageNumber, likesParams.PageSize);
    }

    public async Task<AppUser> GetUserWithLikes(int userId)
    {
        return await _context.Users.Include( l => l.LikedUsers)
        .FirstOrDefaultAsync(u => u.Id == userId);
    }
}
