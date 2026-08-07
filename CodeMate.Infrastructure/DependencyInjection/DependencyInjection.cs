using CodeMate.Application.Common.Interfaces.Security;
using CodeMate.Infrastructure.Authentication.Password;
using Microsoft.Extensions.DependencyInjection;

namespace CodeMate.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}