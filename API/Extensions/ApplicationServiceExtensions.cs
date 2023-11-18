using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddCors();
        services.AddScoped<ITokenService, TokenService>();
        
        //Auto mapper configuration
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        //Access app settings with strongly type property
        services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
        //Photo upload service
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<LogUserActivity>();
        // services.AddScoped<ILikesRepository, LikesRepository>();
        // services.AddScoped<IMessageRepository, MessageRepository>();
        // services.AddScoped<IUserRepository, UserRepository>();
        //Instead of adding single repository to services we can add this.
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddSignalR();
        services.AddSingleton<PresenceTracker>();
        
        return services;
    }
}
