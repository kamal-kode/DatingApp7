using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public UserRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<MemberDto> GetMemberAsync(string username)
    {
        var result= await _context.Users.Where(x => x.UserName == username)
        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
        .SingleOrDefaultAsync();
        return result;
    }

    public async Task<PageList<MemberDto>> GetMembersAsync(UserParams userParams)
    {
        var q= _context.Users.AsQueryable();
        q = q.Where(u => u.UserName != userParams.CurrentUserName);
        q = q.Where(u => u.Gender == userParams.Gender);
        
        //Date of birth filter. Get data with min and max date of birth.
        var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge -1 ));
        var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge ));
        q = q.Where( u => u.DateOfBirth >= minDob && u.DateOfBirth <=maxDob);
        
        q = userParams.OrderBy switch {
            "created" => q.OrderByDescending(u => u.Created),
            _=> q.OrderByDescending(u => u.LastActive)
        };
        var query = q.ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
        .AsNoTracking();

        return await PageList<MemberDto>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);
    }

    public async Task<AppUser> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<AppUser> GetUserByUserNameAsync(string username)
    {
        return await _context.Users.Include(p => p.photos).SingleOrDefaultAsync(x => x.UserName == username);
    }

    public async Task<string> GetUserGender(string username)
    {
       return await _context.Users.Where(u => u.UserName == username)
       .Select(g => g.Gender).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await _context.Users.Include(p => p.photos).ToListAsync();
    }

    public void Update(AppUser user)
    {
        _context.Entry(user).State = EntityState.Modified;
    }
}
