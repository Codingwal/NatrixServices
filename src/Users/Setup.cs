using Microsoft.EntityFrameworkCore;
using NatrixServices.Users.Application.Interfaces;
using NatrixServices.Users.Infrastructure;

namespace NatrixServices.Users;

public static class SetupExtensions
{
    public static void AddUserServices(this IServiceCollection services)
    {
        services.AddDbContext<UserStorage>(options => options.UseSqlite($"Data Source=data/users.db"));
        services.AddScoped<IUserStorage>(sp => sp.GetRequiredService<UserStorage>());
    }
}